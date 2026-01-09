namespace HomeNet.Web.Auth;

public interface IAuthService
{
    Task<bool> LoginAsync(
        HttpContext context, 
        string username, 
        string password);

    Task LogoutAsync(
        HttpContext context);
}
