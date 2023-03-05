using AzerSelling.Models;
using System.ComponentModel.DataAnnotations;

namespace AzerSelling.ViewModels;
public class OrderViewModel
{
    public int ProductId { get; set; }


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

    public Product? Product { get; set; }
}