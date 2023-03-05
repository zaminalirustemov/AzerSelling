using System.ComponentModel.DataAnnotations;

namespace AzerSelling.Models;
public class OrderItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int OrderId { get; set; }

    [StringLength(maximumLength:100)]
    public string Name { get; set; }
    [StringLength(maximumLength: 100)]
    public string? ImageName { get; set; }
    public double CostPrice { get; set; }
    public double SalePrice { get; set; }
    public double DiscountPercent { get; set; }
    public DateTime? CreatedDate { get; set; }

    public Product? Product { get; set; }
    public Order? Order { get; set; }
}