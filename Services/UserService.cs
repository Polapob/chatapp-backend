using chatapp_backend.Dtos;
using chatapp_backend.Models;
using chatapp_backend.Repositories;
using BC = BCrypt.Net.BCrypt;

namespace chatapp_backend.Services;

public interface IUserService
{
    User UpdateUser(UpdateUserDTO updateUserDTO, Guid id);
    User GetUserById(Guid id);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User GetUserById(Guid id)
    {
        User? user = _userRepository.GetUserById(id);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }
        return user;
    }

    public User UpdateUser(UpdateUserDTO updateUserDTO, Guid id)
    {
        User? prevUser = _userRepository.GetUserById(id);
        if (prevUser == null)
        {
            throw new KeyNotFoundException("User not found");
        }
        string hashPassword = BC.EnhancedHashPassword(updateUserDTO.Password);
        User updatedUser = new User
        {
            Id = prevUser.Id,
            FirstName = updateUserDTO.FirstName,
            LastName = updateUserDTO.LastName,
            UserName = updateUserDTO.UserName,
            Email = prevUser.Email,
            Password = hashPassword,
            CreatedAt = prevUser.CreatedAt,
            UpdatedAt = new DateTime()
        };
        _userRepository.UpdateUser(updatedUser);
        return updatedUser;
    }
}