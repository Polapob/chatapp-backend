using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<string> pathIncluded = new List<string>() { "/api/Auth/testMiddleware", "/api/User" };


    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public Task Invoke(HttpContext context)
    {
        var request = context.Request;

        int idx = pathIncluded.FindIndex((path) =>
        {
            if (path.Contains(request.Path) || request.Path.ToString().Contains(path))
            {
                return true;
            }
            return false;
        });

        if (idx == -1)
        {
            return _next(context);
        }

        var accessToken = request.Cookies["accessToken"];
        if (accessToken == null)
        {
            throw new UnauthorizedAccessException();
        }
        string? userId = ValidateToken(accessToken);

        if (userId == null)
        {
            throw new UnauthorizedAccessException();
        }
        context.Request.Headers["userId"] = userId;
        return _next(context);
    }

    private string? ValidateToken(string token)
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
            string userId = jwtToken.Claims.First(x => x.Type == "id").Value;
            return userId;
        }
        catch
        {
            return null;
        }
    }
}

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder AddAuthMiddleware(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder.UseMiddleware<AuthenticationMiddleware>();
    }
}