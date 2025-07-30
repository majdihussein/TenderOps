using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;

namespace TenderOps_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IPartnerService _partnerService;
        private readonly ITokenProvider _tokenProvider;


        public AuthController(IAuthService authService, ITokenProvider tokenProvider, IPartnerService partnerService)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
            _partnerService = partnerService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO obj = new();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(obj);

            if (response != null && response.IsSuccess)
            {
                TokenDTO model = JsonConvert.DeserializeObject<TokenDTO>(Convert.ToString(response.Result)); // الان بدنا نتعامل مع التوكين بداخل الجلسات - session

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(model.AccessToken);

                // انشاء هوية كوكي للمستخدم للتنقل دون الحاجة للدخول مرات عديدة في كل حركة
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme); // انشاء هوية جديدة
                
                identity.AddClaim(new Claim(ClaimTypes.Name, jwt
                    .Claims.FirstOrDefault(u =>u.Type == "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role, jwt
                    .Claims.FirstOrDefault(u =>u.Type == "role").Value));

                var principal = new ClaimsPrincipal(identity); // انشاء كائن جديد من ClaimsPrincipal
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                _tokenProvider.SetToken(model); // تخزين التوكين في الجلسة
                return RedirectToAction("Index", "Home"); // الى الكنترولر هوم ومن ثم الى الاندكس بداخل الهوم
            }
            else
            {
                ModelState.AddModelError("CustomError", response.ErrorMessage.FirstOrDefault());
                return View(obj);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (!User.IsInRole("superadmin"))
            {
                return View("RegisterCustomerPage");
            }

            var roleList = new List<SelectListItem>
            {
                new SelectListItem { Text = SD.Admin, Value = SD.Admin },
                new SelectListItem { Text = SD.superadmin, Value = SD.superadmin },
                new SelectListItem { Text = SD.Customer, Value = SD.Customer },
                new SelectListItem { Text = SD.TestRole, Value = SD.TestRole }

            };
            var partners = await _partnerService.GetAllAsync<List<PartnerDTO>>();    
            ViewBag.RoleList = roleList;
            ViewBag.PartnerList = new SelectList(partners, "Id", "Name");

            return View(); // صفحة التسجيل الكاملة
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDTO obj)
        {
            ViewBag.RoleList = new List<SelectListItem>
    {
        new SelectListItem { Text = SD.Admin, Value = SD.Admin },
        new SelectListItem { Text = SD.superadmin, Value = SD.superadmin },
        new SelectListItem { Text = SD.Customer, Value = SD.Customer },
        new SelectListItem { Text = SD.TestRole, Value = SD.TestRole }
    };

            // جلب قائمة الشركاء
            var partners = await _partnerService.GetAllAsync<List<PartnerDTO>>();
            ViewBag.PartnerList = new SelectList(partners, "Id", "Name", obj.PartnerId);

            // تحقق أولاً من صلاحية البيانات (Model validation)
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            // عيّن قيمة افتراضية للدور لو ما تم اختياره
            if (string.IsNullOrEmpty(obj.Role))
            {
                obj.Role = SD.Customer;
            }

            // محاولة التسجيل
            APIResponse result = await _authService.RegisterAsync<APIResponse>(obj);

            if (result != null && result.IsSuccess)
            {
                return RedirectToAction(nameof(Login));
            }

            // لو فشلت عملية التسجيل (مثلاً التوكين ناقص، أو كلمة المرور ضعيفة...)
            ModelState.AddModelError(string.Empty, "Registration failed. Please check your inputs.");

            return View(obj);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // Sign out from the current session
            var token = _tokenProvider.GetToken();
            await _authService.LogoutAsync<APIResponse>(token);
            _tokenProvider.ClearToken(); // Clear the token from the session
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
