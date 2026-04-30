using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApproveFlow.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ApproveFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly JwtOptions _jwtOptions;
    private readonly IWebHostEnvironment _environment;

    public AuthController(IOptions<JwtOptions> jwtOptions, IWebHostEnvironment environment)
    {
        _jwtOptions = jwtOptions.Value;
        _environment = environment;
    }

    [HttpPost("dev-token")]
    public IActionResult CreateDevelopmentToken([FromBody] DevTokenRequest request)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return BadRequest("userId is required.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.UserId),
            new(ClaimTypes.NameIdentifier, request.UserId),
            new(ClaimTypes.Name, request.UserName),
            new(ClaimTypes.Role, request.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(8);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return Ok(new DevTokenResponse(new JwtSecurityTokenHandler().WriteToken(token), expires));
    }
}

public sealed record DevTokenRequest(string UserId, string UserName = "Dev Admin", string Role = "Administrator");

public sealed record DevTokenResponse(string AccessToken, DateTime ExpiresAtUtc);
