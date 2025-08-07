using Microsoft.AspNetCore.Identity;

namespace MinimalAPIPeliculas.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<IdentityUser> _userManager;

    public UserService(
        IHttpContextAccessor httpContextAccessor,
        UserManager<IdentityUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<IdentityUser?> GetUser()
    {
        var emailClaim = _httpContextAccessor.HttpContext!.User.Claims.Where(x => x.Type == "email").FirstOrDefault();
        if (emailClaim is null)
        {
            return null;
        }

        var email = emailClaim.Value;
        var user = await _userManager.FindByEmailAsync(email);
        return user;
    }
}