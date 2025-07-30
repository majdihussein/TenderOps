using TenderOps_Web.Models.Dto;

namespace TenderOps_Web.Services.IServices
{
    public interface ITokenProvider
    {
        void SetToken(TokenDTO tokenDTO); // تخزين توكين واستعماله في اشياء معينة
        TokenDTO? GetToken(); // اعطيني التوكين اذا موجود والمستخدم مسجل دخول
        void ClearToken(); // مسح التوكين من الجلسة في حالة تسجيل الخروج
    }
}
