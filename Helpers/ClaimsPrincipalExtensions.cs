using System.Security.Claims;

namespace UserManagerApi.Helpers;

public static class ClaimsPrincipalExtensions
{
    // Id пользователя из JWT (claim NameIdentifier). null — если не авторизован.
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(value, out var id) ? id : null;
    }

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(Roles.Admin);
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
}
