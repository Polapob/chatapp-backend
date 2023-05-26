using System.ComponentModel.DataAnnotations.Schema;
namespace chatapp_backend.Models;
[Table("Users")]
public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password
    {
        get; set;
    } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}