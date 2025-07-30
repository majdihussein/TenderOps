using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using TenderOps_API.Data;
using TenderOps_API.Models;
using TenderOps_API.Models.Dto;
using TenderOps_API.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace TenderOps_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private string secretkey;
        public UserRepository(ApplicationDbContext db, IConfiguration configuration
            , UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            secretkey = configuration.GetValue<string>("APISettings:Secret");
        }

        public bool IsUniqueUser(string email)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return true; // اليوزر جديد 
            }
            return false;
        }

        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == loginRequestDTO.Email.ToLower());

            if (user == null)
            {
                return new TokenDTO() { AccessToken = "" };
            }

            bool IsValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (!IsValid)
            {
                return new TokenDTO() { AccessToken = "" };
            }

            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            var accessToken = await GetAccessToken(user, jwtTokenId); // الحصول على التوكين

            var refreshToken = await CreateNewRefreshToken(user.Id, jwtTokenId); // انشاء توكن جديد

            TokenDTO tokenDTO = new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
            return tokenDTO; // ارجاع التوكين مع معلومات المستخدم
        }

        public async Task<UserDTO> Register(RegisteraionRequestDTO registrationRequestDTO)
        {
            // أول شي نتأكد أن الشريك موجود
            var partner = await _db.Partners.FirstOrDefaultAsync(p => p.Id == registrationRequestDTO.PartnerId);
            if (partner == null)
            {
                throw new Exception("Partner not found");
            }

            ApplicationUser user = new()
            {
                UserName = registrationRequestDTO.Email,
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                FullName = registrationRequestDTO.Name,
                RegistrationNumber = registrationRequestDTO.RegistrationNumber,
                PartnerId = registrationRequestDTO.PartnerId // ربط المستخدم بالشريك
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(registrationRequestDTO.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(registrationRequestDTO.Role));
                    }

                    await _userManager.AddToRoleAsync(user, registrationRequestDTO.Role);

                    var userToReturn = await _db.ApplicationUsers
                        .Include(u => u.Partner)
                        .FirstOrDefaultAsync(u => u.Email.ToLower() == registrationRequestDTO.Email.ToLower());

                    return _mapper.Map<UserDTO>(userToReturn);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"User creation failed: {errors}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}");
            }
        }


        private async Task<string> GetAccessToken(ApplicationUser user, string jwtTokenId)
        {
            var roles = await _userManager.GetRolesAsync(user); // الحصول على صلاحيات المستخدم
            var tokenHandler = new JwtSecurityTokenHandler(); // إنشاء المحرك الرئيسي لعملية التوكين
            var key = Encoding.ASCII.GetBytes(secretkey); // تحويل المفتاح إلى بايتات

            // إعداد الـ Claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Aud, "dotnetmastery.com")
    };

            // إضافة جميع الأدوار
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = "https://magicvilla-api.com",
                Audience = "https://test-magic-api.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenStr = tokenHandler.WriteToken(token);
            return tokenStr;
        }


        public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO)
        {
            // find the access and refresh token data
            var existingRefreshToken = _db.RefreshTokens.FirstOrDefault(u => u.Refresh_Token == tokenDTO.RefreshToken);
            if (existingRefreshToken == null)
            {
                return new TokenDTO(); // رجع قيمة فاضية
            }

            // compare data from existing refresh and access token provided and if there is any missmatch then consider it as a fraud
            var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDTO();
            }

            // when someone tries to use not valid token or expired token
            if (!existingRefreshToken.IsValid)
            {
                await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId
                    , existingRefreshToken.JwtTokenId);
                    return new TokenDTO();
            }

            // if just expired then mark as invalid and return empty
            if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDTO();
            }

            // update ole refresh toten and expierd date
            var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId
                , existingRefreshToken.JwtTokenId); // انشاء توكن جديد

            // invoke the exicting refresh token
            await MarkTokenAsInvalid(existingRefreshToken);

            // generate new access token
            var applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == existingRefreshToken.UserId);
            if (applicationUser == null)
            {
                return new TokenDTO();
            }

            var newAccessToken = await GetAccessToken(applicationUser, existingRefreshToken.JwtTokenId); // الحصول على التوكين الجديد
            return new TokenDTO()
            {
                AccessToken = newAccessToken, // التوكين الجديد
                RefreshToken = newRefreshToken // التوكن الجديد
            };
        }

        public async Task RevokeRefreshToken(TokenDTO tokenDTO)
        {
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(_ => _.Refresh_Token == tokenDTO.RefreshToken);

            if (existingRefreshToken == null)
                return;

            // Compare data from Existing refresh and access provided and 
            // if there is any missmatch then we should do nothing with refreshtoken

            var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid)
            {
                return;
            }

            await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
        }

        private async Task<string> CreateNewRefreshToken(string userId, string tokenId)
        {

            RefreshToken refreshToken = new()
            {
                IsValid = true,
                UserId = userId,
                JwtTokenId = tokenId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10), // مدة صلاحية الرفرش توكين
                Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid() // انشاء توكن جديد
            };
            _db.RefreshTokens.Add(refreshToken); // اضافة التوكن الى قاعدة البيانات
            await _db.SaveChangesAsync(); // حفظ التغييرات في قاعدة البيانات
            return refreshToken.Refresh_Token; // ارجاع التوكن الجديد
        }

        private bool GetAccessTokenData(string accessToken, string expectedUserId, string expectedTokenId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);
                var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Jti).Value;
                var userId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value;
                return userId == expectedUserId && jwtTokenId == expectedTokenId;
            }
            catch
            {
                return false;
            }
        }

        private async Task MarkAllTokenInChainAsInvalid(string userId, string tokenId)
        {
            await _db.RefreshTokens.Where(u => u.UserId == userId
                && u.JwtTokenId == tokenId)
                    .ExecuteUpdateAsync(u => u.SetProperty(RefreshToken => RefreshToken.IsValid, false));
        }

        private Task MarkTokenAsInvalid(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            return _db.SaveChangesAsync();
        }


    }
}
