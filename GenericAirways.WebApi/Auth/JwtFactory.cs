
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using GenericAirways.WebApi.Model;
using GenericAirways.Model;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
 using System.Text;

namespace GenericAirways.WebApi.Auth
{
    public class JwtFactory : IJwtFactory<User>
    {
        //private readonly JwtIssuerOptions _jwtOptions;
        private readonly IConfiguration _configuration;

        public JwtFactory(IConfiguration configuration
            //IOptions<JwtIssuerOptions> jwtOptions
            
            )
        {
            /*_jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);*/
            _configuration = configuration;
        }

        public async Task<string> GenerateEncodedToken(User user, ClaimsIdentity identity=null)
        {
           /* var claims = new[]
         {
                 new Claim(JwtRegisteredClaimNames.Sub, userName),
                 new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                 identity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol),
                 identity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id)
             };*/
             var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())//Guid.NewGuid().ToString())
            };

            // Create the JWT security token and encode it.
            /*var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);*/

            var jwt = new JwtSecurityToken
            (
                issuer: _configuration.GetSection("AppConfiguration")["Issuer"],
                audience: _configuration.GetSection("AppConfiguration")["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(60),
                //notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppConfiguration")["SigningKey"])), SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public ClaimsIdentity GenerateClaimsIdentity(User user, string id)
        {
            return new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"), new[]
            {
                new Claim(Constants.Strings.JwtClaimIdentifiers.Id, id),
                new Claim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess)
            });
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        /*private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }*/
    }

    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Rol = "rol", Id = "id";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
            }
        }
    }
}