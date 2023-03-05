using AzerSelling.DAL;
using AzerSelling.Enums;
using AzerSelling.Models;
using AzerSelling.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace AzerSelling.Controllers;
public class CheckoutController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AzerSellingDbContext _azerSellingDbContext;

    public CheckoutController(UserManager<AppUser> userManager,AzerSellingDbContext azerSellingDbContext)
    {
        _userManager = userManager;
        _azerSellingDbContext = azerSellingDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        OrderViewModel orderViewModel = null;
        List<Order> orders = null;
        AppUser member = null;
        if (HttpContext.User.Identity.IsAuthenticated) member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

        Product product = _azerSellingDbContext.Products.Include(x => x.Category).Include(x => x.Company).Include(x => x.ProductImages).Where(x => x.isDeleted == false).FirstOrDefault(x => x.Id == id);
        if (product == null) return NotFound();


        orderViewModel = new OrderViewModel
        {
            Product = product,
            ProductId = product.Id,
            Fullname = member?.Fullname,
            Phonenumber = member?.PhoneNumber,
            Email = member?.Email
        };
        return View(orderViewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(OrderViewModel orderVM)
    {
        Product product = _azerSellingDbContext.Products.Include(x => x.Category).Include(x => x.Company).Include(x => x.ProductImages).Where(x => x.isDeleted == false).FirstOrDefault(x => x.Id == orderVM.ProductId);
        if (product == null) return NotFound();
        orderVM.Product = product;
        if (!ModelState.IsValid) return View(orderVM);


        AppUser member = null;
        if (HttpContext.User.Identity.IsAuthenticated) member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

        Order order = null;

        OrderItem orderItem = new OrderItem
        {
            ProductId = product.Id,
            Product=product,
            Name = product.Name,
            CostPrice=product.CostPrice,
            SalePrice=product.SalePrice,
            DiscountPercent=product.DiscountPercent,
            CreatedDate = DateTime.UtcNow.AddHours(4),
            ImageName = product.ProductImages.FirstOrDefault(x => x.isPoster == true).ImageName,
            
        };

        order = new Order
        {
            Fullname = orderVM.Fullname,
            Email = orderVM.Email,
            Phonenumber = orderVM.Phonenumber,
            Country = orderVM.Country,
            Address = orderVM.Address,
            City = orderVM.City,
            Note = orderVM?.Note,
            TotalPrice=(member is null? product.SalePrice : product.SalePrice - (product.SalePrice * product.DiscountPercent) / 100),
            CreatedDate= DateTime.UtcNow.AddHours(4),
            OrderStatus=OrderStatus.Pending,
            AppUserId = member?.Id,
            OrderItem = orderItem,
            
        };
         orderItem.Order=order;
        
        _azerSellingDbContext.Orders.Add(order);
        _azerSellingDbContext.SaveChanges();
                    
        return RedirectToAction("index","home");
    }
}