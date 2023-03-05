using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AzerSelling.Models;
public class AppUser:IdentityUser
{
    [StringLength(maximumLength: 100)]
    public string Fullname { get; set; }
}
