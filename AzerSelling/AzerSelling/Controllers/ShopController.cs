using AzerSelling.DAL;
using AzerSelling.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzerSelling.Controllers;
public class ShopController : Controller
{
    private readonly AzerSellingDbContext _azerSellingDbContext;

    public ShopController(AzerSellingDbContext azerSellingDbContext)
    {
        _azerSellingDbContext = azerSellingDbContext;
    }
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Detail(int id)
    {
        ViewBag.ProductsOfAzercell = _azerSellingDbContext.Products.Include(x => x.ProductImages).Include(x => x.Company).Where(x => x.isDeleted == false).Where(x => x.Category.isDeleted == false).Where(x => x.Company.isDeleted == false).Where(x => x.Company.Name == "Azercell").Take(9).ToList();
        Product product = _azerSellingDbContext.Products.Include(x => x.Category).Include(x => x.Company).Include(x => x.ProductImages).Where(x=>x.isDeleted==false).FirstOrDefault(x => x.Id == id);
        if (product == null) return NotFound();
        return View(product);
    }
}