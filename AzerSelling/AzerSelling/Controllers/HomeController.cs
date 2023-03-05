
using AzerSelling.DAL;
using AzerSelling.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AzerSelling.Controllers;
public class HomeController : Controller
{
    private readonly AzerSellingDbContext _azerSellingDbContext;

    public HomeController(AzerSellingDbContext azerSellingDbContext)
    {
        _azerSellingDbContext = azerSellingDbContext;
    }
    public IActionResult Index()
    {
        HomeViewModel homeVM = new HomeViewModel
        {
            LatestProducts=_azerSellingDbContext.Products.Include(x=>x.ProductImages).Include(x=>x.Company).Where(x=>x.isDeleted==false).Where(x=>x.Category.isDeleted==false).Where(x=>x.Company.isDeleted==false).Take(8).ToList(),
            ProductsOfAzercell=_azerSellingDbContext.Products.Include(x=>x.ProductImages).Include(x=>x.Company).Where(x=>x.isDeleted==false).Where(x=>x.Category.isDeleted==false).Where(x=>x.Company.isDeleted==false).Where(x=>x.Company.Name=="Azercell").Take(9).ToList(),
            
        };
        return View(homeVM);
    }
}