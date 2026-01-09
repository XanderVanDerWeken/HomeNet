using HomeNet.Core.Modules.Auth.Models;

namespace HomeNet.Web.Auth;

public interface IAuthService
{
    Task SignInAsync(HttpContext context, User user);

    Task LogoutAsync(HttpContext context);
}
