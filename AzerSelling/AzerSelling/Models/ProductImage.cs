using System.ComponentModel.DataAnnotations;

namespace AzerSelling.Models;
public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }


    [StringLength(maximumLength: 100)]
    public string? ImageName { get; set; }
    public bool isPoster { get; set; }

    public Product? Product { get; set; }
}