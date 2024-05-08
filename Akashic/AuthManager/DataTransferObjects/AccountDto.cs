namespace AuthManager.DataTransferObjects;

public class AccountDto
{
    public int Uid { get; set; }
    public string? Email { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Username { get; set; }
    public bool Verified { get; set; }
    public bool Admin { get; set; }
}