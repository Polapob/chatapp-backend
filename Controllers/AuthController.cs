using chatapp_backend.Dtos;
using chatapp_backend.Models;
using chatapp_backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace chatapp_backend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterDTO> _registerDTOValidator;
    private readonly IValidator<LoginDTO> _loginDTOValidator;

    public AuthController(IAuthService authService, IValidator<RegisterDTO> registerDTOValidator, IValidator<LoginDTO> loginDTOValidator)
    {
        _authService = authService;
        _registerDTOValidator = registerDTOValidator;
        _loginDTOValidator = loginDTOValidator;
    }

    [HttpPost("/api/[controller]/register")]
    async public Task<IActionResult> Register(RegisterDTO registerDTO)
    {
        var result = _registerDTOValidator.Validate(registerDTO);
        if (!result.IsValid)
        {
            return new BadRequestObjectResult(new { error = result.Errors.Select((error) => error.ErrorMessage) });
        }
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

    [HttpPost("/api/[controller]/login")]
    public IActionResult Login(LoginDTO loginDTO)
    {
        var result = _loginDTOValidator.Validate(loginDTO);
        if (!result.IsValid)
        {
            return new BadRequestObjectResult(new { error = result.Errors.Select((error) => error.ErrorMessage) });
        }
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

    [HttpGet("/api/[controller]/testMiddleware")]
    public IActionResult TestMiddleWare()
    {
        return Ok("Login successfully with user id = " + Request.Headers["userId"]);
    }


}