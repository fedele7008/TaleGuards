using AuthManager.Abstractions;
using AuthManager.Data;
using AuthManager.Extensions;
using AuthManager.Repositories;
using AuthManager.ServiceLinker;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace AuthManager;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Env.Load($"{AppDomain.CurrentDomain.BaseDirectory}.env");
        
        var server = Environment.GetEnvironmentVariable("AKASHIC_DB_SERVER");
        var port = Environment.GetEnvironmentVariable("AKASHIC_DB_PORT");
        var dbName = Environment.GetEnvironmentVariable("AKASHIC_DB_NAME");
        var user = Environment.GetEnvironmentVariable("AKASHIC_DB_USER");
        var password = Environment.GetEnvironmentVariable("AKASHIC_DB_PASSWORD");
        var connectionString = $"Server={server};Port={port};Database={dbName};Uid={user};Pwd={password};";
        
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.WriteIndented = true;
        });
        builder.Services.AddSingleton<IRsaCryptoService, RsaCryptoService>();
        builder.Services.AddSingleton<ISignalService, SignalService>();
        builder.Services.AddScoped<IAccountRepo, AccountRepo>();
        builder.Services.AddScoped<IServiceRepo, ServiceRepo>();
        builder.Services.AddScoped<IAccessRepo, AccessRepo>();
        builder.Services.AddScoped<ISuspensionLogRepo, SuspensionLogRepo>();
        builder.Services.AddScoped<IJwtUtilities, JwtUtilities>();
        builder.Services.AddDbContext<AkashicDbContext>(options =>
        {
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 3, 0)));
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();
        
        var signalService = app.Services.GetRequiredService<ISignalService>();
        var logger = app.Services.GetRequiredService<ILogger<ServiceLinkingManager>>();
        var rsaManager = app.Services.GetRequiredService<IRsaCryptoService>();
        var tcpPort = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("TcpPort");
        var serviceLinkingManager = new ServiceLinkingManager(tcpPort, signalService, logger, rsaManager);

        var tcp = serviceLinkingManager.Start();
        await Task.WhenAny(app.RunAsync(), tcp);
    }
}
