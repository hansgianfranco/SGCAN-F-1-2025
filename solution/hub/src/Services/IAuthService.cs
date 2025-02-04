using Hub.DTOs;

namespace Hub.Services
{
    public interface IAuthService
    {
        Task<string> Register(RegisterDTO registerDTO);
        Task<string> Login(LoginDTO loginDTO);
    }
}