using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AuthManager.Models;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Access
{
    [Column(TypeName = "INT")] [Required] public int Uid { get; set; }
    [Column(TypeName = "INT")] [Required] public int Sid { get; set; }
    [Column(TypeName = "TINYINT(1)")] [Required] public bool Banned { get; set; }
    [Column(TypeName = "DATETIME(6)")] public DateTime? SuspensionEndAt { get; set; }
    
    [Required] public Account? Account { get; set; }
    [Required] public Service? Service { get; set; }
}
