using Microsoft.AspNetCore.Identity;

namespace MinimalAPIPeliculas.Services;

public interface IUserService
{
    Task<IdentityUser?> GetUser();
}