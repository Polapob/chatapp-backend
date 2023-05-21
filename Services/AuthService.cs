using chatapp_backend.Dtos;
using chatapp_backend.Repositories;
using chatapp_backend.Models;
using BC = BCrypt.Net.BCrypt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace chatapp_backend.Services;

public interface IAuthService
{
    Task<User> Register(RegisterDTO registerDTO);
    Token Login(LoginDTO loginDTO);
    bool Logout(LogoutDTO logoutDTO);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    async public Task<User> Register(RegisterDTO registerDTO)
    {
        if (registerDTO.Password != registerDTO.ConfirmPassword)
        {
            throw new BadHttpRequestException("Confirm password and password mismatch");
        }

        string hashPassword = BC.EnhancedHashPassword(registerDTO.Password);

        return await _userRepository.CreateUser(new User
        {
            Id = new Guid(),
            Email = registerDTO.Email,
            UserName = registerDTO.UserName,
            Password = hashPassword,
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            CreatedAt = new DateTime(),
            UpdatedAt = new DateTime()
        });
    }

    public Token Login(LoginDTO loginDTO)
    {
        User? _user = _userRepository.GetUserByEmail(loginDTO.Email);
        if (_user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }
        if (BC.EnhancedVerify(loginDTO.Password, _user.Password) == false)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var refreshToken = CreateToken(_user, DotNetEnv.Env.GetInt("REFRESH_TOKEN_EXPIRE_IN_MS"));
        var accessToken = CreateToken(_user, DotNetEnv.Env.GetInt("ACCESS_TOKEN_EXPIRE_IN_MS"));
        return new Token { RefreshToken = refreshToken, AccessToken = accessToken };
    }

    public bool Logout(LogoutDTO logoutDTO)
    {
        if (ValidateToken(logoutDTO.RefreshToken) != null || ValidateToken(logoutDTO.AccessToken) != null)
        {
            return true;
        }
        return false;
    }

    private string CreateToken(User user, int timeoutMs)
    {
        List<Claim> claims = new List<Claim>{
            new Claim("id",user.Id.ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DotNetEnv.Env.GetString("JWT_SECRET")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMilliseconds(timeoutMs),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private Guid? ValidateToken(string token)
    {
        if (token == null)
        {
            return null;
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DotNetEnv.Env.GetString("JWT_SECRET")));
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            Guid userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            return userId;
        }
        catch
        {
            return null;
        }
    }
}