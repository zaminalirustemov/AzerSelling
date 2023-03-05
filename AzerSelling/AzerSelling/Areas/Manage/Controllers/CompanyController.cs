using AzerSelling.DAL;
using AzerSelling.Helpers;
using AzerSelling.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AzerSelling.Areas.Manage.Controllers;
[Area("Manage")]
[Authorize(Roles = "SuperAdmin,Admin,Editor")]
public class CompanyController : Controller
{
    private readonly AzerSellingDbContext _azerSellingDbContext;
    private readonly IWebHostEnvironment _environment;

    public CompanyController(AzerSellingDbContext azerSellingDbContext, IWebHostEnvironment environment)
    {
        _azerSellingDbContext = azerSellingDbContext;
        _environment = environment;
    }
    public IActionResult Index(int page = 1)
    {
        var query = _azerSellingDbContext.Companies.Where(x => x.isDeleted == false).OrderByDescending(x => x.CreatedDate).AsQueryable();
        var paginatedList = PaginatedList<Company>.Create(query, 7, page);
        return View(paginatedList);
    }
    //Create-------------------------------------------------------------------------------
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(Company company)
    {
        if (!ModelState.IsValid) return View(company);

        company.CreatedDate = DateTime.UtcNow.AddHours(4);
        _azerSellingDbContext.Companies.Add(company);
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
    //Update-------------------------------------------------------------------------------
    public IActionResult Update(int id)
    {
        Company company = _azerSellingDbContext.Companies.FirstOrDefault(x => x.Id == id);
        if (company == null) return NotFound();

        return View(company);
    }
    [HttpPost]
    public IActionResult Update(Company newCompany)
    {
        Company existCompany = _azerSellingDbContext.Companies.FirstOrDefault(x => x.Id == newCompany.Id);
        if (existCompany == null) return NotFound();
        if (!ModelState.IsValid) return View(newCompany);

        existCompany.Name = newCompany.Name;
        existCompany.UpdatedDate = DateTime.UtcNow.AddHours(4);
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
    //Soft Delete--------------------------------------------------------------------------
    public IActionResult SoftDelete(int id)
    {
        Company company = _azerSellingDbContext.Companies.FirstOrDefault(x => x.Id == id);
        if (company == null) return NotFound();

        company.isDeleted = true;
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }


    //*************************************************************************************
    //*************************************Recycle Bin*************************************
    //*************************************************************************************
    //Deleted Index------------------------------------------------------------------------
    public IActionResult DeletedIndex(int page = 1)
    {
        var query = _azerSellingDbContext.Companies.Where(x => x.isDeleted == true).AsQueryable();
        var paginatedList = PaginatedList<Company>.Create(query, 7, page);
        return View(paginatedList);
    }
    //Restore------------------------------------------------------------------------------
    public IActionResult Restore(int id)
    {
        Company company = _azerSellingDbContext.Companies.FirstOrDefault(x => x.Id == id);
        if (company == null) return NotFound();

        company.isDeleted = false;
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(DeletedIndex));
    }
    //Hard Delete--------------------------------------------------------------------------
    public IActionResult HardDelete(int id)
    {
        Company company = _azerSellingDbContext.Companies.FirstOrDefault(x => x.Id == id);
        if (company == null) return NotFound();
        List<Product> products = _azerSellingDbContext.Products.Include(x => x.ProductImages).Where(x => x.CompanyId == company.Id).ToList();
        foreach (Product product in products)
        {
            foreach (ProductImage productImage in product.ProductImages)
            {
                FileManager.DeleteFile(_environment.WebRootPath, "uploads/product", productImage.ImageName);
            }
        }

        _azerSellingDbContext.Companies.Remove(company);
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(DeletedIndex));
    }
    //All Delete-----------------------------------------------------------------------
    public IActionResult AllDelete()
    {
        List<Company> companies = _azerSellingDbContext.Companies.Where(x => x.isDeleted == true).ToList();
        if (companies.Count == 0) return NotFound();

        foreach (Company company in companies)
        {
            List<Product> products = _azerSellingDbContext.Products.Include(x => x.ProductImages).Where(x => x.CompanyId == company.Id).ToList();
            foreach (Product product in products)
            {
                foreach (ProductImage productImage in product.ProductImages)
                {
                    FileManager.DeleteFile(_environment.WebRootPath, "uploads/product", productImage.ImageName);
                }
            }
        }
        _azerSellingDbContext.Companies.RemoveRange(companies);
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(DeletedIndex));
    }
}