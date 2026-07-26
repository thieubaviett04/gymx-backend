using GymX.Application.Common.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;


namespace GymX.Infrastructure.Services
{

    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public string? Email => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }
}
