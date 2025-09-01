using backend_api_luanvan.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load .env
Env.Load();

// Build connection string từ biến môi trường
string connectionString =
    $"server={Environment.GetEnvironmentVariable("DB_HOST")};" +
    $"database={Environment.GetEnvironmentVariable("DB_DATABASE")};" +
    $"user id={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
    $"password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

builder.Services.AddDbContext<Dbluanvan2Context>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.40-mysql")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
