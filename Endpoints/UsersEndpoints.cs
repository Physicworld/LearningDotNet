using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Filters;
using MinimalAPIPeliculas.Services;
using MinimalAPIPeliculas.Utilities;

namespace MinimalAPIPeliculas.Endpoints;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsers(this RouteGroupBuilder group)
    {
        group.MapPost("/register", RegisterUser)
            .AddEndpointFilter<FilterValidations<UserCredentialsDTO>>();

        group.MapPost("/login", LoginUser)
            .AddEndpointFilter<FilterValidations<UserCredentialsDTO>>();

        group.MapPost("/makeadmin", MakeAdmin)
            .AddEndpointFilter<FilterValidations<EditClaimDTO>>();
        // .RequireAuthorization(policyNames: "isAdmin");

        group.MapPost("/removeadmin", RemoveAdmin)
            .AddEndpointFilter<FilterValidations<EditClaimDTO>>();
        // .RequireAuthorization(policyNames: "isAdmin");

        group.MapGet("/updatetoken", UpdateToken).RequireAuthorization();

        return group;
    }

    static async Task<Results<Ok<AuthenticationResponseDTO>, BadRequest<IEnumerable<IdentityError>>>> RegisterUser(
        UserCredentialsDTO userCredentialsDto,
        [FromServices] UserManager<IdentityUser> userManager,
        IConfiguration configuration
    )
    {
        var user = new IdentityUser
        {
            UserName = userCredentialsDto.Email,
            Email = userCredentialsDto.Email
        };
        var result = await userManager.CreateAsync(user, userCredentialsDto.Password);
        if (result.Succeeded)
        {
            var token = await BuildToken(userCredentialsDto, configuration, userManager);
            return TypedResults.Ok(token);
        }
        else
        {
            return TypedResults.BadRequest(result.Errors);
        }
    }

    static async Task<Results<Ok<AuthenticationResponseDTO>, BadRequest<string>>> LoginUser(
        UserCredentialsDTO userCredentialsDto,
        [FromServices] SignInManager<IdentityUser> signInManager,
        [FromServices] UserManager<IdentityUser> userManager,
        IConfiguration configuration
    )
    {
        var user = await userManager.FindByEmailAsync(userCredentialsDto.Email);
        if (user is null)
        {
            return TypedResults.BadRequest("Incorrect Login");
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, userCredentialsDto.Password, false);
        if (result.Succeeded)
        {
            var token = await BuildToken(userCredentialsDto, configuration, userManager);
            return TypedResults.Ok(token);
        }
        else
        {
            return TypedResults.BadRequest("Incorrect Login");
        }
    }

    static async Task<Results<NoContent, NotFound>> MakeAdmin(
        EditClaimDTO editClaimDto,
        [FromServices] UserManager<IdentityUser> userManager
    )
    {
        var user = await userManager.FindByEmailAsync(editClaimDto.Email);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        await userManager.AddClaimAsync(user, new Claim("isAdmin", "true"));
        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> RemoveAdmin(
        EditClaimDTO editClaimDto,
        [FromServices] UserManager<IdentityUser> userManager
    )
    {
        var user = await userManager.FindByEmailAsync(editClaimDto.Email);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        await userManager.RemoveClaimAsync(user, new Claim("isAdmin", "true"));
        return TypedResults.NoContent();
    }

    public async static Task<Results<Ok<AuthenticationResponseDTO>, NotFound>> UpdateToken(
        IUserService userService,
        IConfiguration configuration,
        [FromServices] UserManager<IdentityUser> userManager)
    {
        var user = await userService.GetUser();
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var userCredentialsDTO = new UserCredentialsDTO
        {
            Email = user.Email!
        };
        var token = BuildToken(userCredentialsDTO, configuration, userManager);
        return TypedResults.Ok(token);
    }

    private async static Task<AuthenticationResponseDTO> BuildToken(
        UserCredentialsDTO userCredentialsDto,
        IConfiguration configuration,
        UserManager<IdentityUser> userManager
    )
    {
        var claims = new List<Claim>
        {
            new Claim("email", userCredentialsDto.Email)
        };

        var user = await userManager.FindByEmailAsync(userCredentialsDto.Email);
        var claimsDB = await userManager.GetClaimsAsync(user);
        claims.AddRange(claimsDB);

        var key = Keys.GetKey(configuration);
        var creds = new SigningCredentials(key.First(), SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddYears(1);
        var securityToken = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );
        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return new AuthenticationResponseDTO
        {
            Token = token,
            Expiration = expiration
        };
    }
}