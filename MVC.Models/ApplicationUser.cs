using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Id property yahan NAHI honi chahiye
        [Required]
        public string Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
    }
}