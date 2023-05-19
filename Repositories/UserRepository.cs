using chatapp_backend.Data;
using chatapp_backend.Models;
namespace chatapp_backend.Repositories;

public interface IUserRepository
{
    Task<User> CreateUser(User user);
    User UpdateUser(User user);
    User GetUserById(Guid id);
    User GetUserByEmail(string email);
    User DeleteUser(Guid id);
}

public class UserRepository
{

    private readonly AppDBContext _appDBContext;
    public UserRepository(AppDBContext appDBContext)
    {
        _appDBContext = appDBContext;
    }

    async public Task<User> CreateUser(User user)
    {
        User? _user = await _appDBContext.AddAsync<User>(user);
        return _user;
    }

    public User UpdateUser(User user)
    {
        User? _user = _appDBContext.Update<User>(user);
        _appDBContext.SaveChanges();
        return _user;
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