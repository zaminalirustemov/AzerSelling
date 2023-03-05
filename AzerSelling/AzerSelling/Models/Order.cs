using AzerSelling.Enums;
using System.ComponentModel.DataAnnotations;

namespace AzerSelling.Models;
public class Order
{
    public int Id { get; set; }
    public string? AppUserId { get; set; }


    [StringLength(maximumLength: 100)]
    public string Fullname { get; set; }
    [StringLength(maximumLength: 25)]
    public string Phonenumber { get; set; }
    [StringLength(maximumLength: 100)]
    public string Email { get; set; }
    [StringLength(maximumLength: 100)]
    public string Country { get; set; }
    [StringLength(maximumLength: 100)]
    public string Address { get; set; }
    [StringLength(maximumLength: 100)]
    public string City { get; set; }
    [StringLength(maximumLength: 500)]
    public string? Note { get; set; }
    public double TotalPrice { get; set; }
    public bool isDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public OrderStatus OrderStatus { get; set; }

    public AppUser? AppUser { get; set; }
    public OrderItem? OrderItem { get; set; } = new OrderItem();
}