namespace AuthManager.DataTransferObjects;

public class TokensDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}