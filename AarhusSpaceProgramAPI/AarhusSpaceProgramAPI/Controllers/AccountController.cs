using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AarhusSpaceProgramAPI.Data;
using AarhusSpaceProgramAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AarhusSpaceProgramAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApiUser> _userManager;

    public AccountController(
        IConfiguration configuration,
        UserManager<ApiUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

[HttpPost("Login")]
public async Task<ActionResult> Login(LoginDto input)
{
    try
    {
        if (!ModelState.IsValid)
        {
            var details = new ValidationProblemDetails(ModelState);
            details.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            details.Status = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(details);
        }

        var user = await _userManager.FindByNameAsync(input.UserName);

        if (user == null || !await _userManager.CheckPasswordAsync(user, input.Password))
        {
            throw new Exception("Invalid login attempt.");
        }

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    _configuration["JWT:SigningKey"] 
                    ?? throw new InvalidOperationException("JWT:SigningKey is not configured.")
                )
            ),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
        };

        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var jwtObject = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddSeconds(3600),
            signingCredentials: signingCredentials
        );

        var jwtString = new JwtSecurityTokenHandler().WriteToken(jwtObject);

       return StatusCode(StatusCodes.Status200OK, jwtString);
    }
    catch (Exception e)
    {
        var exceptionDetails = new ProblemDetails();
        exceptionDetails.Detail = e.Message;
        exceptionDetails.Status = StatusCodes.Status401Unauthorized;
        exceptionDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

        return StatusCode(StatusCodes.Status401Unauthorized, exceptionDetails);
    }
}
}

