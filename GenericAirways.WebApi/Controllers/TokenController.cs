using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using GenericAirways.Model;
using Microsoft.Extensions.Configuration;


namespace GenericAirways.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly IConfiguration _configuration;
        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("token")]
        public IActionResult Post([FromBody]User user)
        {
            if (ModelState.IsValid)
            {
                var userId = GetUserIdFromCredentials(user);
                if (userId == -1)
                {
                    return Unauthorized();
                }

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken
                (
                    issuer: _configuration.GetSection("AppConfiguration")["Issuer"],
                    audience: _configuration.GetSection("AppConfiguration")["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(60),
                    //notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppConfiguration")["SigningKey"])), SecurityAlgorithms.HmacSha256)
                );

                return Ok(new { token = new JwtSecurityTokenHandlerCustom().WriteToken(token) });
            }

            return BadRequest();
        }

        private int GetUserIdFromCredentials(User user)
        {
            var userId = -1;
            if (user.UserName == "wally" /*&& loginViewModel.Password == "SuperSecret"*/)
            {
                userId = 5;
            }

            return userId;
        }
    }
    public class JwtSecurityTokenHandlerCustom : JwtSecurityTokenHandler{
        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var claims = base.ValidateToken(token, validationParameters, out validatedToken);
            Console.WriteLine("AAAAA "+claims);
            return claims;
        }

    }
}
