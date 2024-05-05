using AuthManager.Data;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace AuthManager;

public static class Program
{
    public static void Main(string[] args)
    {
        Env.Load($"{AppDomain.CurrentDomain.BaseDirectory}.env");
        
        var server = Environment.GetEnvironmentVariable("AKASHIC_DB_SERVER");
        var port = Environment.GetEnvironmentVariable("AKASHIC_DB_PORT");
        var dbName = Environment.GetEnvironmentVariable("AKASHIC_DB_NAME");
        var user = Environment.GetEnvironmentVariable("AKASHIC_DB_USER");
        var password = Environment.GetEnvironmentVariable("AKASHIC_DB_PASSWORD");
        var connectionString = $"Server={server};Port={port};Database={dbName};Uid={user};Pwd={password};";
        
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddDbContext<AkashicDbContext>(options =>
        {
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 3, 0)));
        });
        
        var app = builder.Build();
        
        app.MapControllers();
        app.Run();
    }
}