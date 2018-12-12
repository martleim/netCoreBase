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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

using GenericAirways.WebApi.Auth;

namespace GenericAirways.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory<User> _jwtFactory;

        public TokenController(IConfiguration configuration, 
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        IJwtFactory<User> jwtFactory
        )
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            
            /*_userManager.UserValidators.Clear();
            _userManager.PasswordValidators.Clear();*/
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Post([FromBody]User user)
        {
            if (ModelState.IsValid)
            {
                //await _userManager.CreateAsync(user, user.Password);
                //return Ok();
                var result = await _signInManager.PasswordSignInAsync(user.UserName, user.Password, false, lockoutOnFailure: false);
                
                if (result.Succeeded){
                    
                    return Ok(new { token = _jwtFactory.GenerateEncodedToken(user)});
                    //return Ok(new { token = TokenCreator.Create(user,_configuration)});
                    
                }else{
                    return Unauthorized();
                }
                
            }

            return BadRequest();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok();
                }
                BadRequest(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        

    }
    /*public class JwtSecurityTokenHandlerCustom : JwtSecurityTokenHandler{
        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var claims = base.ValidateToken(token, validationParameters, out validatedToken);
            Console.WriteLine("AAAAA "+claims);
            return claims;
        }
    }*/





    
}
