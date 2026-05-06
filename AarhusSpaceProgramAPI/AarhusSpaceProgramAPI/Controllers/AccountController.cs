using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AarhusSpaceProgramAPI.Data;
using AarhusSpaceProgramAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AarhusSpaceProgramAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApiUser> _userManager;
    private readonly SignInManager<ApiUser> _signInManager;

    public AccountController(
        ILogger<AccountController> logger,
        IConfiguration configuration,
        UserManager<ApiUser> userManager,
        SignInManager<ApiUser> signInManager)
    {
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
    }



    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult> Register(RegisterDto input)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var newUser = new ApiUser();

                newUser.UserName = input.Email;
                newUser.Email = input.Email;
                newUser.FullName = input.FullName;
                
                var result = await _userManager.CreateAsync(newUser, input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation(
                    "User {userName} ({email}) has been created.",
                    newUser.UserName, newUser.Email);
                    return StatusCode(201,
                    $"User '{newUser.UserName}' has been created.");
                }
                else
                    throw new Exception(
                    string.Format("Error: {0}", string.Join(" ",
                    result.Errors.Select(e => e.Description))));
            }
            else
            {
                var details = new ValidationProblemDetails(ModelState);
                details.Type =
                "https:/ /tools.ietf.org/html/rfc7231#section-6.5.1";
                details.Status = StatusCodes.Status400BadRequest;
                return new BadRequestObjectResult(details);
            }
        }
        catch (Exception e)
        {
            var exceptionDetails = new ProblemDetails();
            exceptionDetails.Detail = e.Message;
            exceptionDetails.Status =
            StatusCodes.Status500InternalServerError;
            exceptionDetails.Type =
            "https:/ /tools.ietf.org/html/rfc7231#section-6.6.1";
            return StatusCode(
            StatusCodes.Status500InternalServerError,
            exceptionDetails);
        }
    }

    [HttpPost]
    [Route("Login")] 
       public async Task<ActionResult> Login(LoginDto input)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(input.UserName);
                if (user == null || !await _userManager.CheckPasswordAsync(user, input.Password))
                    throw new Exception("Invalid login attempt.");
                else
                {
                    var signingCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"] ?? throw new InvalidOperationException("JWT:SigningKey is not configured."))),
                            SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, user.UserName ?? string.Empty));
                    var userClaims = await _userManager.GetClaimsAsync(user);
                    claims.AddRange(userClaims);

                    var jwtObject = new JwtSecurityToken(
                            issuer: _configuration["JWT:Issuer"],
                            audience: _configuration["JWT:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddSeconds(3600),
                            signingCredentials: signingCredentials);
                    var jwtString = new JwtSecurityTokenHandler()
                    .WriteToken(jwtObject);
                    
                    return StatusCode(StatusCodes.Status200OK, jwtString);
                }
            }
            else
            {
                var details = new ValidationProblemDetails(ModelState);
                details.Type =
                "https:/ /tools.ietf.org/html/rfc7231#section-6.5.1";
                details.Status = StatusCodes.Status400BadRequest;
                return new BadRequestObjectResult(details);
            }
        }
        catch (Exception e)
        {
            var exceptionDetails = new ProblemDetails();
            exceptionDetails.Detail = e.Message;
            exceptionDetails.Status =
            StatusCodes.Status401Unauthorized;
            exceptionDetails.Type =
            "https:/ /tools.ietf.org/html/rfc7231#section-6.6.1";
            return StatusCode(
                StatusCodes.Status401Unauthorized, exceptionDetails);
        }
    }
}

