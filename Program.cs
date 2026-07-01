using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


// Запросы
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
