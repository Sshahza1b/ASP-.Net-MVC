using Microsoft.AspNetCore.Identity;
using MVC.DataAccess.Data;
using MVC.DataAccess.Repository.IRepository;
using MVC.Models;

namespace MVC.DataAccess.Repository
{
    // IdentityUser ko ApplicationUser se replace karein
    public class ApplicationUserRepository : Repository<IdentityUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}