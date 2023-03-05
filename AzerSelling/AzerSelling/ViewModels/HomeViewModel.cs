using AzerSelling.Models;

namespace AzerSelling.ViewModels;
public class HomeViewModel
{
    public List<Product> LatestProducts { get; set; }
    public List<Product> ProductsOfAzercell { get; set; }
}