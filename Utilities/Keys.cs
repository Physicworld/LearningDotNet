using Microsoft.IdentityModel.Tokens;

namespace MinimalAPIPeliculas.Utilities;

public static class Keys
{
    public const string SelfIssuer = "my-app";
    private const string KeySection = "Authentication:Schemes:Bearer:SigningKeys";
    private const string KeySectionEmissor = "Issuer";
    private const string KeySectionValue = "Value";

    public static IEnumerable<SecurityKey> GetKey(IConfiguration configuration) => GetKeys(configuration, SelfIssuer);

    public static IEnumerable<SecurityKey> GetKeys(
        IConfiguration configuration,
        string issuer
    )
    {
        var signingKey = configuration.GetSection(KeySection)
            .GetChildren()
            .SingleOrDefault(key => key[KeySectionEmissor] == issuer);
        if (signingKey is not null && signingKey[KeySectionValue] is string keyValue)
        {
            yield return new SymmetricSecurityKey(Convert.FromBase64String(keyValue));
        }
    }

    public static IEnumerable<SecurityKey> GetAllKeys(
        IConfiguration configuration
    )
    {
        var signingKeys = configuration.GetSection(KeySection)
            .GetChildren();

        foreach (var signingKey in signingKeys)
        {
            if (signingKey[KeySectionValue] is string keyValue)
            {
                yield return new SymmetricSecurityKey(Convert.FromBase64String(keyValue));
            }
        }
    }
}