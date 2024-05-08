using AuthManager.DataTransferObjects;
using AuthManager.Models;

namespace AuthManager.Extensions;

public static class DtoConverters
{
    public static AccountDto ToDto(this Account account) 
        => new() 
        {
            Uid = account.Uid,
            Email = account.Email,
            CreatedAt = account.CreatedAt,
            Username = account.Username,
            Verified = account.Verified,
            Admin = account.Admin,
        };
}