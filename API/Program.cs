using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Data;
using Logic.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<LgDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddSingleton<GameService>();
builder.Services.AddSingleton<CompositionService>();
builder.Services.AddSingleton<GroupService>();
builder.Services.AddSingleton<PlayerService>();
builder.Services.AddSingleton<RoleService>();
builder.Services.AddSingleton<RoleFactoryService>();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();
app.MapControllers();
app.Run();