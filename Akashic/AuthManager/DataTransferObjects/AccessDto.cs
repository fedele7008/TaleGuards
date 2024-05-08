namespace AuthManager.DataTransferObjects;

public class AccessDto
{
    public int Uid { get; set; }
    public int Sid { get; set; }
    public bool Banned { get; set; }
    public DateTime? SuspensionEndAt { get; set; }
}