using chatapp_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace chatapp_backend.Data;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {

    }

    public DbSet<User> Users { set; get; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(DotNetEnv.Env.GetString("DATABASE_CONNECTION_STRING"));
    }
}