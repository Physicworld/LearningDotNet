using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Filters;
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
            var token = BuildToken(userCredentialsDto, configuration);
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
            var token = BuildToken(userCredentialsDto, configuration);
            return TypedResults.Ok(token);
        }
        else
        {
            return TypedResults.BadRequest("Incorrect Login");
        }
    }

    private static AuthenticationResponseDTO BuildToken(
        UserCredentialsDTO userCredentialsDto,
        IConfiguration configuration
    )
    {
        var claims = new List<Claim>
        {
            new Claim("email", userCredentialsDto.Email)
        };

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