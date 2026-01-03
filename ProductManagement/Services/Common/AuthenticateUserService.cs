using System.Security.Claims;

namespace ProductManagement.Services.Common;

public abstract class AuthenticatedUserService
{
    public static bool IsAuthenticated(ClaimsPrincipal principal)
    {
        return  principal.Identity?.IsAuthenticated is true;
    }

    public static Guid GetUserId(ClaimsPrincipal principal)
    {
        if (!IsAuthenticated(principal))
        {
            throw new UnauthorizedAccessException("user is not authenticated");
        }

        string userId = principal.FindFirstValue(ClaimTypes.Name) ?? throw new InvalidOperationException("Name claim not found");
        return Guid.Parse(userId);
    }
}