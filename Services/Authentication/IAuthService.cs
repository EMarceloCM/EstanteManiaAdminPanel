using EstanteManiaWebAssembly.Models;

namespace EstanteManiaWebAssembly.Services.Authentication
{
    public interface IAuthService
    {
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
    }
}
