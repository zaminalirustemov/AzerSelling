using AzerSelling.DAL;
using AzerSelling.Helpers;
using AzerSelling.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing.Drawing2D;

namespace AzerSelling.Areas.Manage.Controllers;
[Area("Manage")]
[Authorize(Roles = "SuperAdmin,Admin,Editor")]
public class CategoryController : Controller
{
    private readonly AzerSellingDbContext _azerSellingDbContext;
    private readonly IWebHostEnvironment _environment;

    public CategoryController(AzerSellingDbContext azerSellingDbContext,IWebHostEnvironment environment)
    {
        _azerSellingDbContext = azerSellingDbContext;
        _environment = environment;
    }
    public IActionResult Index(int page=1)
    {
        var query = _azerSellingDbContext.Categories.Where(x => x.isDeleted == false).OrderByDescending(x => x.CreatedDate).AsQueryable();
        var paginatedList = PaginatedList<Category>.Create(query, 7, page);
        return View(paginatedList);
    }
    //Create-------------------------------------------------------------------------------
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (!ModelState.IsValid) return View(category);

        category.CreatedDate = DateTime.UtcNow.AddHours(4);
        _azerSellingDbContext.Categories.Add(category);
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
    //Update-------------------------------------------------------------------------------
    public IActionResult Update(int id)
    {
        Category category = _azerSellingDbContext.Categories.FirstOrDefault(x => x.Id == id);
        if (category == null) return NotFound();

        return View(category);
    }
    [HttpPost]
    public IActionResult Update(Category newCategory)
    {
        Category existCategory = _azerSellingDbContext.Categories.FirstOrDefault(x => x.Id == newCategory.Id);
        if (existCategory == null) return NotFound();
        if (!ModelState.IsValid) return View(newCategory);

        existCategory.Name = newCategory.Name;
        existCategory.UpdatedDate = DateTime.UtcNow.AddHours(4);
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
    //Soft Delete--------------------------------------------------------------------------
    public IActionResult SoftDelete(int id)
    {
        Category category = _azerSellingDbContext.Categories.FirstOrDefault(x => x.Id == id);
        if (category == null) return NotFound();

        category.isDeleted = true;
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }



    //*************************************************************************************
    //*************************************Recycle Bin*************************************
    //*************************************************************************************
    //Deleted Index------------------------------------------------------------------------
    public IActionResult DeletedIndex(int page = 1)
    {
        var query = _azerSellingDbContext.Categories.Where(x => x.isDeleted == true).AsQueryable();
        var paginatedList = PaginatedList<Category>.Create(query, 7, page);
        return View(paginatedList);
    }
    //Restore------------------------------------------------------------------------------
    public IActionResult Restore(int id)
    {
        Category category = _azerSellingDbContext.Categories.FirstOrDefault(x => x.Id == id);
        if (category == null) return NotFound();

        category.isDeleted = false;
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(DeletedIndex));
    }
    //Hard Delete--------------------------------------------------------------------------
    public IActionResult HardDelete(int id)
    {
        Category category = _azerSellingDbContext.Categories.FirstOrDefault(x => x.Id == id);
        if (category == null) return NotFound();

        List<Product> products = _azerSellingDbContext.Products.Include(x=>x.ProductImages).Where(x => x.CategoryId == category.Id).ToList();
        foreach (Product product in products)
        {
            foreach (ProductImage productImage in product.ProductImages)
            {
                FileManager.DeleteFile(_environment.WebRootPath, "uploads/product", productImage.ImageName);
            }
        }

        _azerSellingDbContext.Categories.Remove(category);
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(DeletedIndex));
    }
    //All Delete-----------------------------------------------------------------------
    public IActionResult AllDelete()
    {
        List<Category> categories = _azerSellingDbContext.Categories.Where(x => x.isDeleted == true).ToList();
        if (categories.Count == 0) return NotFound();

        foreach (Category category in categories)
        {
            List<Product> products = _azerSellingDbContext.Products.Include(x => x.ProductImages).Where(x => x.CategoryId == category.Id).ToList();
            foreach (Product product in products)
            {
                foreach (ProductImage productImage in product.ProductImages)
                {
                    FileManager.DeleteFile(_environment.WebRootPath, "uploads/product", productImage.ImageName);
                }
            }
        }
        _azerSellingDbContext.Categories.RemoveRange(categories);
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(DeletedIndex));
    }
}