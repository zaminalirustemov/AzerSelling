using AzerSelling.DAL;
using AzerSelling.Helpers;
using AzerSelling.Models;
using AzerSelling.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace AzerSelling.Controllers;
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IWebHostEnvironment _environment;
    private readonly AzerSellingDbContext _azerSellingDbContext;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment environment,AzerSellingDbContext azerSellingDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
        _azerSellingDbContext = azerSellingDbContext;
    }
    //Register-----------------------------------------------------------------------------------------------------
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(MemberRegisterViewModel memberRegisterVM)
    {
        if (!ModelState.IsValid) return View();
        AppUser appUser = null;

        appUser = await _userManager.FindByNameAsync(memberRegisterVM.Username);
        if (appUser is not null)
        {
            ModelState.AddModelError("Username", "Already exist!");
            return View();
        }

        appUser = await _userManager.FindByEmailAsync(memberRegisterVM.Email);
        if (appUser is not null)
        {
            ModelState.AddModelError("Email", "Already exist!");
            return View();
        }

        if (memberRegisterVM.PhoneNumber.Substring(0, 3)!="010" && memberRegisterVM.PhoneNumber.Substring(0, 3) != "050" && memberRegisterVM.PhoneNumber.Substring(0, 3) != "051")
        {
            ModelState.AddModelError("PhoneNumber", "You can register only with Azercel numbers");
            return View();
        }

        appUser = new AppUser
        {
            Fullname = memberRegisterVM.Fullname,
            UserName = memberRegisterVM.Username,
            Email = memberRegisterVM.Email,
            PhoneNumber = memberRegisterVM.PhoneNumber
        };


        var result = await _userManager.CreateAsync(appUser, memberRegisterVM.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        string link = Url.Action(nameof(Verify), "Account", new { email = appUser.Email, token = token }, Request.Scheme, Request.Host.ToString());

        MailExtension.SendMessage(appUser.Email, "Verify Email", link);

        await _userManager.AddToRoleAsync(appUser, "Member");


        return RedirectToAction("index", "home");
    }


    public async Task<IActionResult> Verify(string email, string token)
    {
        AppUser user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }
        await _userManager.ConfirmEmailAsync(user, token);
        await _signInManager.SignInAsync(user, true);
        return RedirectToAction("index", "Home");
    }

    //Log in-------------------------------------------------------------------------------------------------------
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(MemberLoginViewModel memberLoginVM)
    {
        if (!ModelState.IsValid) return View();
        AppUser user = await _userManager.FindByNameAsync(memberLoginVM.UserName);
        if (user is null)
        {
            ModelState.AddModelError("", "Username or password is false");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(user, memberLoginVM.Password, false, false);

        if (!result.Succeeded)
        {
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please verify email");
                return View();

            }
            ModelState.AddModelError("", "Username or password is false");
            return View();
        }

        return RedirectToAction("index", "home");
    }
    //Log out------------------------------------------------------------------------------------------------------
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("index", "home");
    }
}