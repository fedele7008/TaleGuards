using AuthManager.Abstractions;
using AuthManager.Data;
using AuthManager.Models;

namespace AuthManager.Repositories;

public class AccessRepo(AkashicDbContext context) : RepoBase<Access>(context), IAccessRepo
{
    // No extra functionality required
}