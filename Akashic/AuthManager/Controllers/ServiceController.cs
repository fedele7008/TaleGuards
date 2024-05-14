using System.Security.Cryptography;
using System.Text.Json;
using AuthManager.Abstractions;
using AuthManager.DataTransferObjects;
using AuthManager.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;

namespace AuthManager.Controllers;

[ApiController]
[Route("Akashic/[controller]")]
public class ServiceController(
    ILogger<AuthController> logger,
    IJwtUtilities jwtManager,
    IRsaCryptoService rsaManager,
    IAccountRepo accountRepo,
    IServiceRepo serviceRepo,
    IAccessRepo accessRepo,
    ISuspensionLogRepo logRepo) : Controller
{
    [HttpPost("register")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [Produces("application/json")]
    [RequireHttps]
    public async Task<IActionResult> Register([FromHeader(Name = "Api-Key")] string apiKey, [FromBody] ServiceRegistrationDto serviceInfo)
    {
        Env.Load($"{AppDomain.CurrentDomain.BaseDirectory}.env");
        var key = Environment.GetEnvironmentVariable("AKASHIC_API_KEY");

        if (apiKey != key)
        {
            logger.LogWarning("Service registration failed:\nInvalid API Key");
            return BadRequest(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.ServiceRegistrationInvalidApiKey,
                Result = "Service registration failed",
                Detail = "Invalid API Key"
            }));
        }

        var service = await serviceRepo.GetServiceByNameAsync(serviceInfo.Name);
        if (service is null)
        {
            logger.LogInformation("Service registration: transaction initiated");
            await using var transaction = await serviceRepo.BeginTransactionAsync();

            try
            {
                string secretKey;
                using (var randomNumberGenerator = RandomNumberGenerator.Create())
                {
                    var randomBytes = new byte[48];
                    randomNumberGenerator.GetBytes(randomBytes);
                    secretKey = Convert.ToBase64String(randomBytes);
                }

                if (secretKey.Length != 64)
                {
                    logger.LogCritical("Service registration failed:\nInvalid secret key generated");
                    return StatusCode(500, JsonSerializer.Serialize(new ResponseDto()
                    {
                        Type = ResponseType.ServiceRegistrationFailedInternalError,
                        Result = "Service registration failed",
                        Detail = "Invalid secret key generation"
                    }));
                }

                await serviceRepo.CreateAsync(new Service()
                {
                    Name = serviceInfo.Name,
                    SecretKey = secretKey,
                    ConnectionUrl = serviceInfo.Url
                });

                await serviceRepo.SaveChangesAsync();
                logger.LogInformation("Service successfully registered");

                service = await serviceRepo.GetServiceByNameAsync(serviceInfo.Name)
                    ?? throw new NullReferenceException("Service is null");
                var accounts = await accountRepo.GetAllAccountsAsync();

                foreach (var account in accounts)
                {
                    var newAccess = new Access()
                    {
                        Uid = account.Uid,
                        Sid = service.Sid
                    };
                    await accessRepo.CreateAsync(newAccess);
                }
                
                logger.LogInformation("User service access granted");
                await accessRepo.SaveChangesAsync();
                await transaction.CommitAsync();
                logger.LogInformation("Service registration pushed to database, ending transaction");
            }
            catch (OperationCanceledException ex)
            {
                await transaction.RollbackAsync();
                logger.LogError("Service registration cancelled:\n{Message}", ex);
                return BadRequest(JsonSerializer.Serialize(new ResponseDto()
                {
                    Type = ResponseType.ServiceRegistrationCancelled,
                    Result = "Service registration cancelled",
                    Detail = ex.ToString(),
                    Data = ex
                }));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogCritical("Service registration failed:\n{Message}", ex);
                return BadRequest(JsonSerializer.Serialize(new ResponseDto()
                {
                    Type = ResponseType.ServiceRegistrationFailed,
                    Result = "Service registration failed",
                    Detail = ex.ToString(),
                    Data = ex
                }));
            }
        }
        else
        {
            logger.LogInformation("Service already registered (Sid: {Sid})", service.Sid);
        }
        
        var tcpPort = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("TcpPort");

        return Ok(JsonSerializer.Serialize(new ResponseDto()
        {
            Type = ResponseType.ServiceRegistrationSuccessful,
            Result = "Service successfully registered",
            Data = new ServiceRegistrationResponseDto()
            {
                Sid = service.Sid,
                SecretKey = service.SecretKey!,
                EncryptionKey = rsaManager.GetPublicKey(),
                ServiceLinkerTcpPort = tcpPort,
                ValidationToken = jwtManager.GenerateServiceValidationToken(service.Sid, service.Name!, key)
            }
        }));
    }
}
