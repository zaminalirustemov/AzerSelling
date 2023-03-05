using System.ComponentModel.DataAnnotations;

namespace AzerSelling.Areas.Manage.ViewModels;
public class AdminLoginViewModel
{
    [Required]
    [StringLength(maximumLength: 50)]
    public string Username { get; set; }
    [Required]
    [StringLength(maximumLength: 20, MinimumLength = 8), DataType(DataType.Password)]
    public string Password { get; set; }
}