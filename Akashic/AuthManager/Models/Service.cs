using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AuthManager.Models;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Service
{
    [Column(TypeName = "INT")] [Key] public int Sid { get; set; }
    [Column(TypeName = "VARCHAR(255)")] [Required] public string? Name { get; set; }
    [Column(TypeName = "CHAR(64)")] [Required] public string? SecretKey { get; set; }
    
    // DO NOT USE IEnumerable FOR FOLLOWING Lists. IT WILL CONFLICT WITH EntityFramework core
    public List<Account> Accounts { get; } = [];
    public List<Access> Accesses { get; } = [];
    public List<SuspensionLog> SuspensionLogs { get; } = [];
}