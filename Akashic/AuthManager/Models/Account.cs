using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthManager.Models;

public class Account
{
    [Column(TypeName = "INT")] [Key] public int Uid { get; set; }
    [Column(TypeName = "VARCHAR(255)")] [Required] public string? Email { get; set; }
    [Column(TypeName = "CHAR(64)")] [Required] public string? PasswordHash { get; set; }
    [Column(TypeName = "DATETIME(6)")] [Required] public DateTime? CreatedAt { get; set; }
    [Column(TypeName = "VARCHAR(255)")] [Required] public string? Username { get; set; }
    [Column(TypeName = "TINYINT(1)")] [Required] public bool Verified { get; set; }
    [Column(TypeName = "TINYINT(1)")] [Required] public bool Admin { get; set; }
    public IEnumerable<Service> Services { get; } = [];
    public IEnumerable<Access> Accesses { get; } = [];
}
