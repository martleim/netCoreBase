using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GenericAirways.Model;
using Microsoft.AspNetCore.Identity;

namespace GenericAirways.WebApi
{
    public class TokenCreator{

        public static string Create(User user,IConfiguration configuration){

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())//Guid.NewGuid().ToString())
                /*
                
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                 identity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol),
                 identity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id)

                 */
            };

            var token = new JwtSecurityToken
            (
                issuer: configuration.GetSection("AppConfiguration")["Issuer"],
                audience: configuration.GetSection("AppConfiguration")["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(60),
                //notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppConfiguration")["SigningKey"])), SecurityAlgorithms.HmacSha256)
            );
            return  new JwtSecurityTokenHandler().WriteToken(token);
            //return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });

        }

    }
}