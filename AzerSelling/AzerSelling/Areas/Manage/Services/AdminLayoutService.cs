using AzerSelling.Models;
using Microsoft.AspNetCore.Identity;

namespace AzerSelling.Areas.Manage.Services;
public class AdminLayoutService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppUser> _userManager;

    public AdminLayoutService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }
    public async Task<AppUser> GetUser()
    {
        string name = _httpContextAccessor.HttpContext.User.Identity.Name;
        AppUser appUser = await _userManager.FindByNameAsync(name);
        return appUser;
    }
}