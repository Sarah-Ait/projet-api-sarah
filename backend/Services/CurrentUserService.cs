using System.Security.Claims;
using backend.Constants;
using backend.Exceptions;
using backend.Interfaces;
namespace backend.Services;



public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                throw new UnauthorizedException("Utilisateur non authentifié.");
            }

            return int.Parse(userIdClaim);
        }
    }

    public string Role
    {
        get
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(roleClaim))
            {
                throw new UnauthorizedException("Rôle utilisateur introuvable.");
            }

            return roleClaim;
        }
    }

    public bool IsAdmin => Role == Roles.Admin;
}