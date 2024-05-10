using AuthManager.Abstractions;
using AuthManager.DataTransferObjects;
using AuthManager.Extensions;
using AuthManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthManager.Controllers;

[ApiController]
[Route("Akashic/[controller]")]
public class AuthController(ILogger<AuthController> logger, IJwtUtilities jwtManager, IAccountRepo accountRepo, IServiceRepo serviceRepo, IAccessRepo accessRepo, ISuspensionLogRepo logRepo) : Controller
{
    [HttpGet("status")]
    [ProducesResponseType(200, Type = typeof(string))]
    [Produces("application/json")]
    public IActionResult StatusCheck()
    {
        return Ok(new { Result = "Connection to Akashic Authentication Server is Successful"});
    }

    [HttpPost("register")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [Produces("application/json")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto user, [FromQuery] int? serviceId = null)
    {
        if (!user.Email.IsValidEmail())
        {
            logger.LogError("User registration failed:\nInvalid Email");
            return BadRequest(new { Result = "Invalid Email"});
        }

        if (!user.Password.IsValidPassword())
        {
            logger.LogError("User registration failed:\nInvalid Password");
            return BadRequest(new { Result = "Invalid Password" });
        }

        if (!user.Username.IsValidUsername())
        {
            logger.LogError("User registration failed:\nInvalid Username");
            return BadRequest(new { Result = "Invalid Username"});
        }

        await using var transaction = await accountRepo.BeginTransactionAsync();
        logger.LogInformation("User registration: transaction initiated");

        try
        {
            var emailExists = await accountRepo.CheckEmailExistsAsync(user.Email);

            if (emailExists)
            {
                await transaction.RollbackAsync();
                logger.LogError("User registration failed:\nEmail already exists");
                return BadRequest(new { Result = "Email already exists"});
            }
            
            var usernameExists = await accountRepo.CheckUsernameExistsAsync(user.Username);

            if (usernameExists)
            {
                await transaction.RollbackAsync();
                logger.LogError("User registration failed:\nUsername already exists");
                return BadRequest(new { Result = "Username already exists"});
            }
            
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await accountRepo.CreateAsync(new Account()
            {
                Email = user.Email,
                PasswordHash = passwordHash,
                Username = user.Username,
            });
            
            await accountRepo.SaveChangesAsync();
            logger.LogInformation("User successfully registered");

            var account = await accountRepo.GetAccountByEmailAsync(user.Email);
            var serviceIds = await serviceRepo.GetAllServiceIdsAsync();

            foreach (var sid in serviceIds)
            {
                logger.LogDebug("Granting access to service: {Sid}", sid);
                
                var newAccess = new Access()
                {
                    Uid = account!.Uid,
                    Sid = sid
                };
                await accessRepo.CreateAsync(newAccess);
            }
            
            logger.LogInformation("User service access granted");

            string? accessToken = null;
            string? refreshToken = null;
            string? serviceName = null;
            if (serviceId is not null)
            {
                var service = await serviceRepo.GetByIdAsync(serviceId.Value);
                if (service is not null)
                {
                    serviceName = service.Name;
                    var tokens = jwtManager.GenerateJwtTokens(account!.Uid, account.Email!,
                        account.Username!, (DateTime)account.CreatedAt!, account.Verified, account.Admin,
                        service.SecretKey!);
                    accessToken = tokens.AccessToken;
                    refreshToken = tokens.RefreshToken;
                }
                else
                {
                    logger.LogWarning("Given service ID does not exist: {Sid}", serviceId.Value);
                }
            }
            
            await accountRepo.SaveChangesAsync();
            await transaction.CommitAsync();
            logger.LogInformation("User registration pushed to database, ending transaction");
            
            return Ok(new
            {
                Result = "User successfully registered", 
                Detail = new
                {
                    Account = account!.ToDto(),
                    TargetService = serviceName,
                    AcessToken = accessToken,
                    RefreshToken = refreshToken
                }
            });
        }
        catch (OperationCanceledException ex)
        {
            await transaction.RollbackAsync();
            logger.LogError("User registration cancelled:\n{Message}", ex);
            return BadRequest(new { Result = ex.ToString() });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError("User registration failed:\n{Message}", ex);
            return BadRequest(new { Result = ex.ToString() });
        }
    }
}