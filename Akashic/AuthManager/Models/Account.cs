using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AuthManager.Models;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Account
{
    [Column(TypeName = "INT")] [Key] public int Uid { get; set; }
    [Column(TypeName = "VARCHAR(255)")] [Required] public string? Email { get; set; }
    [Column(TypeName = "CHAR(64)")] [Required] public string? PasswordHash { get; set; }
    [Column(TypeName = "DATETIME(6)")] [Required] public DateTime? CreatedAt { get; set; }
    [Column(TypeName = "VARCHAR(255)")] [Required] public string? Username { get; set; }
    [Column(TypeName = "TINYINT(1)")] [Required] public bool Verified { get; set; }
    [Column(TypeName = "TINYINT(1)")] [Required] public bool Admin { get; set; }
    
    // DO NOT USE IEnumerable FOR FOLLOWING Lists. IT WILL CONFLICT WITH EntityFramework core
    public List<Service> Services { get; } = [];
    public List<Access> Accesses { get; } = [];
    public List<SuspensionLog> SuspensionLogs { get; } = [];
    public List<SuspensionLog> ActionLogs { get; } = [];
}
