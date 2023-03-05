using AzerSelling.DAL;
using AzerSelling.Helpers;
using AzerSelling.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing.Drawing2D;

namespace AzerSelling.Areas.Manage.Controllers;
[Area("Manage")]
[Authorize(Roles = "SuperAdmin,Admin,Editor")]
public class ProductController : Controller
{
    private readonly AzerSellingDbContext _azerSellingDbContext;
    private readonly IWebHostEnvironment _environment;

    public ProductController(AzerSellingDbContext azerSellingDbContext,IWebHostEnvironment environment)
    {
        _azerSellingDbContext = azerSellingDbContext;
        _environment = environment;
    }
    public IActionResult Index(int page=1)
    {
        var query =_azerSellingDbContext.Products.Include(x=>x.Category).Include(x=>x.Company).Include(x=>x.ProductImages).Where(x=>x.Category.isDeleted==false).Where(x=>x.Company.isDeleted==false).Where(x=>x.isDeleted==false).OrderByDescending(x => x.CreatedDate).AsQueryable();
        var paginatedList = PaginatedList<Product>.Create(query, 5, page);

        return View(paginatedList);
    }
    //Create-------------------------------------------------------------------------------
    public IActionResult Create()
    {
        ViewBag.Categories = _azerSellingDbContext.Categories.Where(x => x.isDeleted == false).ToList();
        ViewBag.Companies = _azerSellingDbContext.Companies.Where(x => x.isDeleted == false).ToList();

        return View();
    }
    [HttpPost]
    public IActionResult Create(Product product)
    {
        List<Category> categories = _azerSellingDbContext.Categories.Where(x => x.isDeleted == false).ToList();
        List<Company> companies = _azerSellingDbContext.Companies.Where(x => x.isDeleted == false).ToList();

        ViewBag.Categories = categories;
        ViewBag.Companies = companies;
        if (!ModelState.IsValid) return View(product);
        if (categories.Count == 0)
        {
            ModelState.AddModelError("CategoryId", "Category is required");
            return View(product);
        }
        if (companies.Count == 0)
        {
            ModelState.AddModelError("CompanyId", "Company is required");
            return View(product);
        }
        if (product.CostPrice <= 0)
        {
            ModelState.AddModelError("CostPrice", "The given value cannot be less than 0");
            return View(product);
        }
        if (product.SalePrice <= 0)
        {
            ModelState.AddModelError("SalePrice", "The given value cannot be less than 0");
            return View(product);
        }
        if (product.DiscountPercent < 0 || product.DiscountPercent>100)
        {
            ModelState.AddModelError("DiscountPercent", "It should take a value between 0-100");
            return View(product);
        }

        //Poster Image--------------------------
        if (product.PosterImageFile is null)
        {
            ModelState.AddModelError("PosterImageFile", "Poster image is required");
            return View(product);
        }
        if (product.PosterImageFile.ContentType != "image/jpeg" && product.PosterImageFile.ContentType != "image/png")
        {
            ModelState.AddModelError("PosterImageFile", "The file you upload must be in jpeg or png format.");
            return View(product);
        }
        if (product.PosterImageFile.Length > 2097152)
        {
            ModelState.AddModelError("PosterImageFile", "The size of your uploaded file must be less than 2 MB.");
            return View(product);
        }
        ProductImage posterimage = new ProductImage
        {
            Product = product,
            ImageName = FileManager.SaveFile(_environment.WebRootPath, "uploads/product", product.PosterImageFile),
            isPoster = true
        };
        _azerSellingDbContext.ProductImages.Add(posterimage);
        //Gallery----------------------------------
        if (product.ImageFiles is not null)
        {
            foreach (IFormFile image in product.ImageFiles)
            {
                if (image.ContentType != "image/jpeg" && image.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFiles", "The file you upload must be in jpeg or png format.");
                    return View(product);
                }
                if (image.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFiles", "The size of your uploaded file must be less than 2 MB.");
                    return View(product);
                }
                ProductImage productImage = new ProductImage
                {
                    Product = product,
                    ImageName = FileManager.SaveFile(_environment.WebRootPath, "uploads/product", image),
                    isPoster = false
                };
                _azerSellingDbContext.ProductImages.Add(productImage);
            }
        }

        product.CreatedDate = DateTime.UtcNow.AddHours(4);
        _azerSellingDbContext.Products.Add(product);
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
    //Update----------------------------------------------------------------------------------------
    public IActionResult Update(int id)
    {
        ViewBag.Categories = _azerSellingDbContext.Categories.Where(x => x.isDeleted == false).ToList();
        ViewBag.Companies = _azerSellingDbContext.Companies.Where(x => x.isDeleted == false).ToList();
        Product product = _azerSellingDbContext.Products.Include(x => x.Category).Include(x => x.Company).Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);

        if (product == null) return NotFound();

        return View(product);
    }
    [HttpPost]
    public IActionResult Update(Product newProduct)
    {
        List<Category> categories = _azerSellingDbContext.Categories.Where(x => x.isDeleted == false).ToList();
        List<Company> companies = _azerSellingDbContext.Companies.Where(x => x.isDeleted == false).ToList();

        ViewBag.Categories = categories;
        ViewBag.Companies = companies;
        Product existProduct = _azerSellingDbContext.Products.Include(x => x.Category).Include(x => x.Company).Include(x => x.ProductImages).FirstOrDefault(x => x.Id == newProduct.Id);
        if (existProduct == null) return NotFound();
        if (!ModelState.IsValid) return View(existProduct);
        if (categories.Count == 0)
        {
            ModelState.AddModelError("CategoryId", "Category is required");
            return View(existProduct);
        }
        if (companies.Count == 0)
        {
            ModelState.AddModelError("CompanyId", "Company is required");
            return View(existProduct);
        }
        if (newProduct.CostPrice <= 0)
        {
            ModelState.AddModelError("CostPrice", "The given value cannot be less than 0");
            return View(existProduct);
        }
        if (newProduct.SalePrice <= 0)
        {
            ModelState.AddModelError("SalePrice", "The given value cannot be less than 0");
            return View(existProduct);
        }
        if (newProduct.DiscountPercent < 0 || newProduct.DiscountPercent > 100)
        {
            ModelState.AddModelError("DiscountPercent", "It should take a value between 0-100");
            return View(existProduct);
        }
        //Poster Image-------------------------------------------------------
        if (newProduct.PosterImageFile is not null)
        {
            if (newProduct.PosterImageFile.ContentType != "image/jpeg" && newProduct.PosterImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("PosterImageFile", "The file you upload must be in jpeg or png format.");
                return View(newProduct);
            }
            if (newProduct.PosterImageFile.Length > 2097152)
            {
                ModelState.AddModelError("PosterImageFile", "The size of your uploaded file must be less than 2 MB.");
                return View(newProduct);
            }
            FileManager.DeleteFile(_environment.WebRootPath, "uploads/product", existProduct.ProductImages.FirstOrDefault(x => x.isPoster == true).ImageName);
            _azerSellingDbContext.ProductImages.Remove(existProduct.ProductImages.FirstOrDefault(x => x.isPoster == true));

            ProductImage posterimage = new ProductImage
            {
                Product = newProduct,
                ImageName = FileManager.SaveFile(_environment.WebRootPath, "uploads/product", newProduct.PosterImageFile),
                isPoster = true
            };
            existProduct.ProductImages.Add(posterimage);
        }
        //Gallery----------------------------------
        if (newProduct.ProductImagesIds is not null)
        {
            foreach (var image in existProduct.ProductImages.Where(x => !newProduct.ProductImagesIds.Contains(x.Id) && x.isPoster == false))
            {
                FileManager.DeleteFile(_environment.WebRootPath, "uploads/product", image.ImageName);
            }
            existProduct.ProductImages?.RemoveAll(x => !newProduct.ProductImagesIds.Contains(x.Id) && x.isPoster == false);
        }
        else
        {
            foreach (var image in existProduct.ProductImages.Where(x => x.isPoster == false))
            {
                FileManager.DeleteFile(_environment.WebRootPath, "uploads/product", image.ImageName);
            }
            existProduct.ProductImages.RemoveAll(x => x.isPoster == false);
        }


        if (newProduct.ImageFiles is not null)
        {
            foreach (IFormFile image in newProduct.ImageFiles)
            {
                if (image.ContentType != "image/jpeg" && image.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFiles", "The file you upload must be in jpeg or png format.");
                    return View(newProduct);
                }
                if (image.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFiles", "The size of your uploaded file must be less than 2 MB.");
                    return View(newProduct);
                }
                ProductImage productImage = new ProductImage
                {
                    ProductId = newProduct.Id,
                    ImageName = FileManager.SaveFile(_environment.WebRootPath, "uploads/product", image),
                    isPoster = false
                };
                existProduct.ProductImages.Add(productImage);
            }
        }

        existProduct.CategoryId = newProduct.CategoryId;
        existProduct.CompanyId = newProduct.CompanyId;
        existProduct.Name = newProduct.Name;
        existProduct.CostPrice = newProduct.CostPrice;
        existProduct.SalePrice = newProduct.SalePrice;
        existProduct.DiscountPercent = newProduct.DiscountPercent;
        existProduct.IsAvailable = newProduct.IsAvailable;
        existProduct.Title = newProduct.Title;
        existProduct.Description = newProduct.Description;
        existProduct.UpdatedDate = DateTime.UtcNow.AddHours(4);
        _azerSellingDbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
    //Soft Delete-----------------------------------------------------------------------------------
    public IActionResult SoftDelete(int id)
    {
        Product product = _azerSellingDbContext.Products.Include(x => x.Category).Include(x => x.Company).Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);
        if (product == null) return NotFound();

        product.isDeleted = true;
        _azerSellingDbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }



    //*************************************************************************************
    //*************************************Recycle Bin*************************************
    //*************************************************************************************
    //Deleted Index------------------------------------------------------------------------
    public IActionResult DeletedIndex(int page = 1)
    {
        var query = _azerSellingDbContext.Products.Include(x => x.Category).Include(x => x.Company).Include(x => x.ProductImages).Where(x => x.Category.isDeleted == false).Where(x => x.Company.isDeleted == false).Where(x => x.isDeleted == true).OrderByDescending(x => x.CreatedDate).AsQueryable();
        var paginatedList = PaginatedList<Product>.Create(query, 5, page);

        return View(paginatedList);
    }
    //Restore---------------------------------------------------------------------------------
    public IActionResult Restore(int id)
    {
        Product product = _azerSellingDbContext.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);
        if (product == null) return NotFound();

        product.isDeleted = false;
        _azerSellingDbContext.SaveChanges();
        return RedirectToAction(nameof(DeletedIndex));
    }
    //Hard Delete-----------------------------------------------------------------------------
    public IActionResult HardDelete(int id)
    {
        Product product = _azerSellingDbContext.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);
        if (product == null) return NotFound();

        foreach (ProductImage productImage in product.ProductImages) FileManager.DeleteFile(_environment.WebRootPath, "uploads/product", productImage.ImageName);

        _azerSellingDbContext.Products.Remove(product);
        _azerSellingDbContext.SaveChanges();
        return RedirectToAction(nameof(DeletedIndex));
    }
    //All Delete-----------------------------------------------------------------------
    public IActionResult AllDelete()
    {
        List<Product> products = _azerSellingDbContext.Products.Include(x => x.ProductImages).Where(x => x.isDeleted == true).ToList();
        if (products.Count == 0) return NotFound();

        foreach (Product product in products)
        {
            foreach (ProductImage productImage in product.ProductImages)
            {
                FileManager.DeleteFile(_environment.WebRootPath, "uploads/product", productImage.ImageName);
            }
        }

        _azerSellingDbContext.Products.RemoveRange(products);
        _azerSellingDbContext.SaveChanges();

        return Ok();
    }
}