using AzerSelling.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzerSelling.ViewComponents;
public class AzercellProductsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(List<Product> products)
    {
        return View(products);
    }
}
