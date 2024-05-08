using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AuthManager.Models;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class SuspensionLog
{
    [Column(TypeName = "INT")] [Key] public int Lid { get; set; }
    [Column(TypeName = "VARCHAR(64)")] [Required] public string? Type { get; set; }
    [Column(TypeName = "DATETIME(6)")] [Required] public DateTime? LoggedAt { get; set; }
    [Column(TypeName = "DATETIME(6)")] public DateTime? SuspensionEndAt{ get; set; }
    [Column(TypeName = "VARCHAR(5000)")] [Required] public string? Reason { get; set; }
    [Column(TypeName = "VARCHAR(1000)")] public string? Comment { get; set; }
    
    [Required] public Account? AssigneeAccount { get; set; }
    public Account? AssignerAccount { get; set; }
    [Required] public Service? Service { get; set; }
}