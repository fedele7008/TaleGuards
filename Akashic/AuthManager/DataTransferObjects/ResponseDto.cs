namespace AuthManager.DataTransferObjects;

public enum ResponseType
{
    StatusCheckSuccessful = 0,
    UsernameAvailable,
    UsernameTaken,
    UserRegistrationInvalidEmail,
    UserRegistrationInvalidPassword,
    UserRegistrationInvalidUsername,
    UserRegistrationDuplicateEmail,
    UserRegistrationDuplicateUsername,
    UserRegistrationCancelled,
    UserRegistrationFailed,
    UserRegistrationSuccessful,
    UserLoginInvalidServiceId,
    UserLoginFailed,
    UserLoginBannedFromService,
    UserLoginSuspendedFromService,
    UserLoginSuccessful,
    ServiceRegistrationInvalidApiKey,
    ServiceRegistrationFailedInternalError,
    ServiceRegistrationCancelled,
    ServiceRegistrationFailed,
    ServiceRegistrationSuccessful
}

public record ResponseDto
{
    public ResponseType Type { get; set; }
    public string Result { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public record ServiceAccessDto
{
    public AccountDto Account { get; set; } = null!;
    public string TargetService { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}

public record ServiceRegistrationResponseDto
{
    public int Sid { get; set; }
    public string SecretKey { get; set; } = string.Empty;
    public string EncryptionKey { get; set; } = string.Empty;
    public int ServiceLinkerTcpPort { get; set; }
    public string ValidationToken { get; set; } = string.Empty;
}