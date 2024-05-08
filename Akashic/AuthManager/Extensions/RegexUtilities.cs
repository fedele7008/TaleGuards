using System.Globalization;
using System.Text.RegularExpressions;

namespace AuthManager.Extensions;

public static partial class RegexUtilities
{
    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
    private static partial Regex StrongPasswordRegex();
    
    [GeneratedRegex(@"^[a-zA-Z0-9]+$")]
    private static partial Regex ValidUsernameRegex();
    
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            string DomainMapper(Match match)
            {
                var idn = new IdnMapping();
                var domainName = idn.GetAscii(match.Groups[2].Value);
                return match.Groups[1].Value + domainName;
            }

            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));
        }
        catch (RegexMatchTimeoutException e)
        {
            return false;
        }
        catch (ArgumentException e)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public static bool IsValidPassword(this string password)
    {
        return !string.IsNullOrWhiteSpace(password) && StrongPasswordRegex().IsMatch(password);
    }
    
    public static bool IsValidUsername(this string username)
    {
        return !string.IsNullOrWhiteSpace(username) && ValidUsernameRegex().IsMatch(username);
    }
}
