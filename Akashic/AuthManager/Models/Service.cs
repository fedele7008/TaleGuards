using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthManager.Models;

public class Service
{
    [Column(TypeName = "INT")] [Key] public int Sid { get; set; }
    [Column(TypeName = "VARCHAR(255)")] [Required] public string? Name { get; set; }
    [Column(TypeName = "CHAR(64)")] [Required] public string? SecretKey { get; set; }
    public IEnumerable<Account> Accounts { get; } = [];
    public IEnumerable<Access> Accesses { get; } = [];
}