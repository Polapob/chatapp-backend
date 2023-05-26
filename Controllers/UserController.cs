using chatapp_backend.Dtos;
using chatapp_backend.Models;
using chatapp_backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;


namespace chatapp_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<UpdateUserDTO> _updateUserDTOValidator;

    public UserController(IUserService userService, IValidator<UpdateUserDTO> updateUserDTOValidator)
    {
        _userService = userService;
        _updateUserDTOValidator = updateUserDTOValidator;
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
        var result = _updateUserDTOValidator.Validate(updateUserDTO);
        if (!result.IsValid)
        {
            return new BadRequestObjectResult(new { error = result.Errors.Select((error) => error.ErrorMessage) });
        }

        User user = _userService.UpdateUser(updateUserDTO, guid);
        return NoContent();
    }
}