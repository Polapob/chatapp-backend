using chatapp_backend.Dtos;
using chatapp_backend.Models;
using chatapp_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace chatapp_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }


    [HttpGet("/api/[controller]/{id}")]
    public ActionResult GetUserById(string id)
    {
        var success = Guid.TryParse(id, out Guid guid);
        if (!success)
        {
            throw new BadHttpRequestException("Invalid format of ID");
        }
        var user = _userService.GetUserById(guid);
        return Ok(new
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
            CreateAt = user.CreatedAt,
            UpdateAt = user.UpdatedAt,
        });
    }
    [HttpGet("/api/[controller]/profile")]
    public ActionResult GetMyProfile()
    {
        string? id = Request.Headers["userId"];
        var success = Guid.TryParse(id, out Guid guid);
        if (!success)
        {
            throw new BadHttpRequestException("Invalid format of ID");
        }
        var user = _userService.GetUserById(guid);
        return Ok(new
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
            CreateAt = user.CreatedAt,
            UpdateAt = user.UpdatedAt,
        });
    }

    [HttpPut("/api/[controller]")]
    public IActionResult UpdateUser(UpdateUserDTO updateUserDTO)
    {
        string? id = Request.Headers["userId"];
        var success = Guid.TryParse(id, out Guid guid);
        if (!success)
        {
            throw new BadHttpRequestException("Invalid format of ID");
        }
        User user = _userService.UpdateUser(updateUserDTO, guid);
        return NoContent();
    }
}