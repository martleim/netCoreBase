using System.Security.Claims;
using System.Threading.Tasks;

namespace GenericAirways.WebApi.Auth
{
    public interface IJwtFactory<TUser>
    {
        Task<string> GenerateEncodedToken(TUser user, ClaimsIdentity identity=null);
        ClaimsIdentity GenerateClaimsIdentity(TUser user, string id);
    }
}