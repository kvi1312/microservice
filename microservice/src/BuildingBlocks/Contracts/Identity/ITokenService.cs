using Shared.DTOS.Identity;

namespace Contracts.Identity;

public interface ITokenService
{
    TokenResponse GetToken(TokenRequest request);
}
