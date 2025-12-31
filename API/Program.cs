using Microsoft.EntityFrameworkCore;
using Data;
using Logic.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<LgDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddSingleton<GameService>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();