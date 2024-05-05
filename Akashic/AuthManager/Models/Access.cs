using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthManager.Models;

public class Access
{
    [Column(TypeName = "TINYINT(1)")] [Required] public bool Banned { get; set; }
    [Column(TypeName = "DATETIME(6)")] public DateTime? SuspensionEndAt { get; set; }
}
