using AuthManager.Models;

namespace AuthManager.DataTransferObjects;

public class SuspensionLogDto
{
    public int Lid { get; set; }
    public string? Type { get; set; }
    public DateTime? LoggedAt { get; set; }
    public DateTime? SuspensionEndAt { get; set; }
    public string? Reason { get; set; }
    public string? Comment { get; set; }
    public Account? AssigneeAccount { get; set; }
    public Account? AssignerAccount { get; set; }
    public Service? Service { get; set; }
}