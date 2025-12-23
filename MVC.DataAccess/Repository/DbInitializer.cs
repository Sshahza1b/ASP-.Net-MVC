using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC.DataAccess.Data;
using MVC.DataAccess.Repository.IRepository;

namespace MVC.DataAccess.Repository
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception) { }

            // 1. Roles banayein agar nahi hain
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
            }

            // 2. Admin User check karein Email se (Role se nahi)
            var userInDb = _db.Users.FirstOrDefault(u => u.Email == "Admin@gmail.com");
            if (userInDb == null)
            {
                var user = new IdentityUser
                {
                    UserName = "Admin@gmail.com",
                    Email = "Admin@gmail.com",
                    EmailConfirmed = true
                };

                _userManager.CreateAsync(user, "Admin!123").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            }
        }
    }
}