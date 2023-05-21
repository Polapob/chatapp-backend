using System.Net;
using chatapp_backend.Dtos;
using chatapp_backend.Models;
using chatapp_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace chatapp_backend.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("/register")]
    async public Task<IActionResult> Register(RegisterDTO registerDTO)
    {
        Console.WriteLine("registerDTO =", registerDTO);
        User user = await _authService.Register(registerDTO);
        return CreatedAtAction(nameof(Register), new
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }


    [HttpPost("/login")]
    public IActionResult Login(LoginDTO loginDTO)
    {
        Token token = _authService.Login(loginDTO);

        Response.Cookies.Append("refreshToken", token.RefreshToken, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddMilliseconds(DotNetEnv.Env.GetInt("REFRESH_TOKEN_EXPIRE_IN_MS")),
            HttpOnly = true,
        });
        Response.Cookies.Append("accessToken", token.AccessToken, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddMilliseconds(DotNetEnv.Env.GetInt("ACCESS_TOKEN_EXPIRE_IN_MS")),
            HttpOnly = true,
        });
        return Ok("Login successfully");
    }


}