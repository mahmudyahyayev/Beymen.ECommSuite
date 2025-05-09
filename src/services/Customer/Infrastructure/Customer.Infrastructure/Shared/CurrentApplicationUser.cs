using System.Security.Claims;
using BuildingBlocks.Abstractions.Domain;
using Microsoft.AspNetCore.Http;

namespace Customer.Infrastructure.Shared;

public class CurrentApplicationUser : ICurrentUser<long, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentApplicationUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long UserId
    {
        get
        {
            long userId = 0;

            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var subject = _httpContextAccessor.HttpContext.User.Claims
                                                              .Where(x => x.Type == ClaimTypes.NameIdentifier)
                                                              .Select(x => x.Value)
                                                              .FirstOrDefault();
                var hasUserId = long.TryParse(subject, out userId);
            }

            return userId;
        }
    }


    public string UserName
    {
        get
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var subject = _httpContextAccessor.HttpContext.User.Claims
                                                              .Where(x => x.Type == "username")
                                                              .Select(x => x.Value)
                                                              .FirstOrDefault();

                return subject == null ? string.Empty : subject;
            }

            return string.Empty;
        }
    }

    public string GetCurrentUser()
    {
        if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            var subject = _httpContextAccessor.HttpContext.User.Claims
                                                          .Where(x => x.Type == "username")
                                                          .Select(x => x.Value)
                                                          .FirstOrDefault();

            return subject == null ? string.Empty : subject;
        }

        return string.Empty;
    }
}
