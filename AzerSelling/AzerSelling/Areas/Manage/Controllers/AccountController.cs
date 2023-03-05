using AzerSelling.Areas.Manage.ViewModels;
using AzerSelling.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AzerSelling.Areas.Manage.Controllers;
[Area("Manage")]
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    //Login--------------------------------------------------------------------------------------------------------------------------
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(AdminLoginViewModel adminLoginVM)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "The size of the password cannot be smaller than 8");
            return View(adminLoginVM);
        }
        AppUser appUser = await _userManager.FindByNameAsync(adminLoginVM.Username);
        if (appUser == null)
        {
            ModelState.AddModelError("", "Username or password is invalid");
            return View(adminLoginVM);
        }

        var result = await _signInManager.PasswordSignInAsync(appUser, adminLoginVM.Password, false, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username or password is invalid");
            return View(adminLoginVM);
        }


        return RedirectToAction("index", "dashboard");
    }
    //Logout--------------------------------------------------------------------------------------------------------------------------
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}