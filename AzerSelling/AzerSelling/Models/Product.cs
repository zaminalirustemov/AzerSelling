using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzerSelling.Models;
public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int CompanyId { get; set; }

    [StringLength(maximumLength: 100)]
    public string Name { get; set; }
    public double CostPrice { get; set; }
    public double SalePrice { get; set; }
    public double DiscountPercent { get; set; }
    [StringLength(maximumLength: 200)]
    public string Title { get; set; }
    [StringLength(maximumLength: 500)]
    public string Description { get; set; }
    public bool IsAvailable { get; set; }
    public bool isDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public Category? Category { get; set; }
    public Company? Company { get; set; }
    public List<OrderItem>? OrderItems { get; set; }
    public List<ProductImage>? ProductImages { get; set; }
    [NotMapped]
    public IFormFile? PosterImageFile { get; set; }
    [NotMapped]
    public List<IFormFile>? ImageFiles { get; set; }
    [NotMapped]
    public List<int>? ProductImagesIds { get; set; }

}