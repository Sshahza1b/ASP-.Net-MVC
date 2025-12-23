using MVC.Models;
using Microsoft.AspNetCore.Identity;

namespace MVC.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<IdentityUser>
    {
    }
}