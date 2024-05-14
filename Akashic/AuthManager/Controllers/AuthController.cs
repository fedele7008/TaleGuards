using System.Text.Json;
using AuthManager.Abstractions;
using AuthManager.DataTransferObjects;
using AuthManager.Extensions;
using AuthManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthManager.Controllers;

[ApiController]
[Route("Akashic/[controller]")]
public class AuthController(
    ILogger<AuthController> logger,
    IJwtUtilities jwtManager,
    IRsaCryptoService rsaManager,
    IAccountRepo accountRepo,
    IServiceRepo serviceRepo,
    IAccessRepo accessRepo,
    ISuspensionLogRepo logRepo) : Controller
{
    [HttpGet("status")]
    [ProducesResponseType(200, Type = typeof(string))]
    [Produces("application/json")]
    public IActionResult StatusCheck()
    {
        var response = new ResponseDto()
        {
            Type = ResponseType.StatusCheckSuccessful,
            Result = "Connection to Akashic Authentication Server is Successful",
        };
        return Ok(JsonSerializer.Serialize(response));
    }

    [HttpGet("username-availability")]
    [ProducesResponseType(200)]
    [Produces("application/json")]
    public async Task<IActionResult> CheckUsername([FromQuery] string username)
    {
        var exist = await accountRepo.CheckUsernameExistsAsync(username);
        return Ok(JsonSerializer.Serialize(new ResponseDto()
        {
            Type = exist ? ResponseType.UsernameTaken : ResponseType.UsernameAvailable,
            Result = exist ? "Username Taken" : "Username Available"
        }));
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
            return BadRequest(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.UserRegistrationInvalidEmail,
                Result = "Invalid Email",
                Detail = "Must follow email format"
            }));
        }

        if (!user.Password.IsValidPassword())
        {
            logger.LogError("User registration failed:\nInvalid Password");
            return BadRequest(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.UserRegistrationInvalidPassword,
                Result = "Invalid Password",
                Detail = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase, one number, and one special character"
            }));
        }

        if (!user.Username.IsValidUsername())
        {
            logger.LogError("User registration failed:\nInvalid Username");
            return BadRequest(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.UserRegistrationInvalidUsername,
                Result = "Invalid Username",
                Detail = "Username can only contain alphabet letters and numbers"
            }));
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
                return BadRequest(JsonSerializer.Serialize(new ResponseDto()
                {
                    Type = ResponseType.UserRegistrationDuplicateEmail,
                    Result = "Email already exists"
                }));
            }
            
            var usernameExists = await accountRepo.CheckUsernameExistsAsync(user.Username);

            if (usernameExists)
            {
                await transaction.RollbackAsync();
                logger.LogError("User registration failed:\nUsername already exists");
                return BadRequest(JsonSerializer.Serialize(new ResponseDto()
                {
                    Type = ResponseType.UserRegistrationDuplicateUsername,
                    Result = "Username already exists"
                }));
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

            return Ok(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.UserRegistrationSuccessful,
                Result = "User successfully registered",
                Data = new ServiceAccessDto()
                {
                    Account = account!.ToDto(),
                    TargetService = serviceName!,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            }));
        }
        catch (OperationCanceledException ex)
        {
            await transaction.RollbackAsync();
            logger.LogError("User registration cancelled:\n{Message}", ex);
            return BadRequest(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.UserRegistrationCancelled,
                Result = "User registration cancelled",
                Detail = ex.ToString(),
                Data = ex
            }));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogCritical("User registration failed:\n{Message}", ex);
            return BadRequest(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.UserRegistrationFailed,
                Result = "User registration failed",
                Detail = ex.ToString(),
                Data = ex
            }));
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [Produces("application/json")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginCredentials, [FromQuery] int serviceId)
    {
        var service = await serviceRepo.GetByIdAsync(serviceId);
        if (service is null)
        {
            logger.LogError("Service not found");
            return BadRequest(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.UserLoginInvalidServiceId,
                Result = "Service not found",
                Detail = $"Requested Sid: {serviceId}",
            }));
        }
        
        var emailExists = await accountRepo.CheckEmailExistsAsync(loginCredentials.Email);
        if (!emailExists)
        {
            logger.LogError("User login failed");
            return Unauthorized(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.UserLoginFailed,
                Result = "Login not successful"
            }));
        }
        
        var account = await accountRepo.GetAccountByEmailAsync(loginCredentials.Email) 
                      ?? throw new NullReferenceException("Account is null");

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginCredentials.Password, account.PasswordHash);
        if (!isPasswordValid)
        {
            logger.LogError("User login failed");
            return Unauthorized(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.UserLoginFailed,
                Result = "Login not successful"
            }));
        }

        var access = await accountRepo.GetAccountAccessByIdAsync(serviceId, account.Uid) 
                     ?? throw new NullReferenceException("Access is null");

        if (access.Banned)
        {
            logger.LogError("User login failed");
            return Unauthorized(JsonSerializer.Serialize(new ResponseDto()
            {
                Type = ResponseType.UserLoginBannedFromService,
                Result = "Login not successful",
                Detail = $"User is permanently banned from requested Sid: {serviceId}"
            }));
        }

        if (access.SuspensionEndAt is not null)
        {
            if (access.SuspensionEndAt < DateTime.UtcNow)
            {
                await accountRepo.UnsuspendAccountByIdAsync(serviceId, account.Uid);
                await accountRepo.SaveChangesAsync();
            }
            else
            {
                logger.LogError("User login failed");
                return Unauthorized(JsonSerializer.Serialize(new ResponseDto()
                {
                    Type = ResponseType.UserLoginSuspendedFromService,
                    Result = "Login not successful",
                    Detail = $"User is temporarily banned from requested Sid: {serviceId}\nSuspension ends at {access.SuspensionEndAt.ToString()}"
                }));
            }
        }
        
        var tokens = jwtManager.GenerateJwtTokens(account.Uid, account.Email!,
            account.Username!, (DateTime)account.CreatedAt!, account.Verified, account.Admin,
            service.SecretKey!);
        
        logger.LogInformation("User \"{username}\" ({uid} - {email}) successfully logged into {serviceName}", account.Username, account.Uid, account.Email, service.Name);
        return Ok(JsonSerializer.Serialize(new ResponseDto()
        {
            Type = ResponseType.UserLoginSuccessful,
            Result = "Login successful",
            Data = new ServiceAccessDto()
            {
                Account = account.ToDto(),
                TargetService = service.Name!,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken
            }
        }));
    }
}