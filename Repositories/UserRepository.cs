using chatapp_backend.Data;
using chatapp_backend.Models;
namespace chatapp_backend.Repositories;

public interface IUserRepository
{
    Task<User> CreateUser(User user);
    User UpdateUser(User user);
    User? GetUserById(Guid id);
    User? GetUserByEmail(string email);
    void DeleteUser(Guid id);
}

public class UserRepository : IUserRepository
{

    private readonly AppDBContext _appDBContext;
    public UserRepository(AppDBContext appDBContext)
    {
        _appDBContext = appDBContext;
    }

    async public Task<User> CreateUser(User user)
    {

        User? _checkUser = GetUserByEmail(user.Email);

        if (_checkUser != null)
        {
            throw new BadHttpRequestException("This email is already used.");

        }
        await _appDBContext.Users.AddAsync(user);
        await _appDBContext.SaveChangesAsync();
        return user;
    }

    public User UpdateUser(User user)
    {
        var prevUser = _appDBContext.Users.First(u => u.Id == user.Id);
        prevUser.FirstName = user.FirstName;
        prevUser.LastName = user.LastName;
        prevUser.UserName = user.UserName;
        prevUser.Password = user.Password;
        prevUser.UpdatedAt = user.UpdatedAt;
        _appDBContext.SaveChanges();
        return user;
    }

    public User? GetUserById(Guid id)
    {
        return _appDBContext.Users.Where(u => u.Id == id).FirstOrDefault();
    }

    public User? GetUserByEmail(string email)
    {
        return _appDBContext.Users.Where(u => u.Email == email).FirstOrDefault();
    }

    public void DeleteUser(Guid id)
    {
        User? user = GetUserById(id);
        if (user == null)
        {
            throw new BadHttpRequestException("Invalid User id");
        }

        _appDBContext.Remove<User>(user);
        _appDBContext.SaveChanges();
    }

}