using Contracts.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Configurations;
using Shared.DTOS.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(JwtSettings settings)
    {
        _settings = settings;
    }

    public TokenResponse GetToken(TokenRequest request)
    {
        var token = GenerateJwt();
        return new TokenResponse(token);
    }

    private string GenerateJwt() => GenerateEncryptedToken(GetSigningCredentials());

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secretKey = Encoding.UTF8.GetBytes(_settings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials)
    {
        var token = new JwtSecurityToken(
            //expires: DateTime.UtcNow.AddMinutes(30)
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}
