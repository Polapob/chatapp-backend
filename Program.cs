using chatapp_backend.Data;
using chatapp_backend.Repositories;
using chatapp_backend.Services;
using chatapp_backend.Validators;
using chatapp_backend.Dtos;
using FluentValidation;
using chatapp_backend.Middlewares;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDBContext>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IValidator<RegisterDTO>, RegisterDTOValidator>();
builder.Services.AddScoped<IValidator<LoginDTO>, LoginDTOValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // Swagger open at /swagger/index.html
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.AddGlobalErrorHandler();
app.AddAuthMiddleware();
app.UseAuthorization();
app.MapControllers();
app.Run();
