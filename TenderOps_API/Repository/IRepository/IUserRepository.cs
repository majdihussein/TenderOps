using TenderOps_API.Models;
using TenderOps_API.Models.Dto;

namespace TenderOps_API.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisteraionRequestDTO registrationRequestDTO);
        Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO); // تحديث التوكين

        Task RevokeRefreshToken(TokenDTO tokenDTO);
    }
}
