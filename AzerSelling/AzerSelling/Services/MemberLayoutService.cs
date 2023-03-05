using AzerSelling.Models;
using Microsoft.AspNetCore.Identity;

namespace AzerSelling.Services;
public class MemberLayoutService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MemberLayoutService(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AppUser> GetUser()
    {
        AppUser user = null;
        if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            string name = _httpContextAccessor.HttpContext.User.Identity.Name;

            user = await _userManager.FindByNameAsync(name);
            return user;
        }
        return null;
    }
}