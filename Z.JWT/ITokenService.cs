using System.Security.Claims;

namespace Z.JWT
{
    public interface ITokenService
    {
        string BuildToken(IEnumerable<Claim> claims,JWTOptions options);
    }
}