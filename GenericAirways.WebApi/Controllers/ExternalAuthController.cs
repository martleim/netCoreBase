using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using GenericAirways.WebApi.Model;
using GenericAirways.Model;
using GenericAirways.WebApi.Auth;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GenericAirways.WebApi.Controllers
{
  [Route("api/[controller]/[action]")]
  public class ExternalAuthController : Controller
  {
    //private readonly ApplicationDbContext _appDbContext;
    private readonly UserManager<User> _userManager;
    private readonly FacebookAuthSettings _fbAuthSettings;
    private readonly IJwtFactory<User> _jwtFactory;
    //private readonly JwtIssuerOptions _jwtOptions;
    private static readonly HttpClient Client = new HttpClient();

    public ExternalAuthController(IOptions<FacebookAuthSettings> fbAuthSettingsAccessor, 
    UserManager<User> userManager, 
    //ApplicationDbContext appDbContext, 
    IJwtFactory<User> jwtFactory 
    //,IOptions<JwtIssuerOptions> jwtOptions
    ){
      _fbAuthSettings = fbAuthSettingsAccessor.Value;
      _userManager = userManager;
      //_appDbContext = appDbContext;
      _jwtFactory = jwtFactory;
      //_jwtOptions = jwtOptions.Value;
    }

    // POST api/externalauth/facebook
    [HttpPost]
    public async Task<IActionResult> Facebook([FromBody]FacebookAuthViewModel model)
    {
      // 1.generate an app access token
      var appAccessTokenResponse = await Client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_fbAuthSettings.AppId}&client_secret={_fbAuthSettings.AppSecret}&grant_type=client_credentials");
      var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);
      // 2. validate the user access token
      var userAccessTokenValidationResponse = await Client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={model.AccessToken}&access_token={appAccessToken.AccessToken}");
      var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

      if (!userAccessTokenValidation.Data.IsValid)
      {
        return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid facebook token.", ModelState));
      }

      // 3. we've got a valid token so we can request user data from fb
      var userInfoResponse = await Client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={model.AccessToken}");
      var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);

      // 4. ready to create the local user account (if necessary) and jwt
      var user = await _userManager.FindByEmailAsync(userInfo.Email);

      if (user == null)
      {
        var newUser = new User
        {
          FirstName = userInfo.FirstName,
          LastName = userInfo.LastName,
          FacebookId = userInfo.Id,
          Email = userInfo.Email,
          UserName = userInfo.Email,
          //PictureUrl = userInfo.Picture.Data.Url
        };

        var result = await _userManager.CreateAsync(newUser, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));

        if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

        //await _appDbContext.Customers.AddAsync(new Customer { IdentityId = user.Id, Location = "",Locale = userInfo.Locale,Gender = userInfo.Gender});
        //await _appDbContext.SaveChangesAsync();
      }

      // generate the jwt for the local user...
      var localUser = await _userManager.FindByNameAsync(userInfo.Email);

      if (localUser==null)
      {
        return BadRequest(Errors.AddErrorToModelState("login_failure", "Failed to create local user account.", ModelState));
      }

      //var jwt = await Tokens.GenerateJwt(_jwtFactory.GenerateClaimsIdentity(localUser.UserName, localUser.Id), _jwtFactory, localUser.UserName, _jwtOptions, new JsonSerializerSettings {Formatting = Formatting.Indented});
  
      var identity = _jwtFactory.GenerateClaimsIdentity(user, user.Id+"");  
         var response = new
        {
          id = identity.Claims.Single(c => c.Type == "id").Value,
          auth_token = await _jwtFactory.GenerateEncodedToken(user, identity),
          expires_in = 60000000// (int)jwtOptions.ValidFor.TotalSeconds
        };


      return new OkObjectResult(response);
    }
  }


  public static class Errors
  {
  public static ModelStateDictionary AddErrorsToModelState(IdentityResult identityResult, ModelStateDictionary modelState) {
    foreach (var e in identityResult.Errors)
    {
    modelState.TryAddModelError(e.Code, e.Description);
    }

    return modelState;
  }

  public static ModelStateDictionary AddErrorToModelState(string code, string description, ModelStateDictionary modelState)
  {
  modelState.TryAddModelError(code, description);
  return modelState;
  }
  }
}