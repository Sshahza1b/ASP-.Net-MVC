using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVC.Models;

namespace MVC.DataAccess.Repository.IRepository
{
    // Yahan OrderDetail (singular) hona chahiye
    public interface IOrderDetailsRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
    }
}