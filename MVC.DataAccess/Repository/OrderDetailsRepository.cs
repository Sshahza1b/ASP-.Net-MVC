using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVC.DataAccess.Data;
using MVC.DataAccess.Repository.IRepository;
using MVC.Models;

namespace MVC.DataAccess.Repository
{
    // OrderDetail (singular) use karein
    public class OrderDetailsRepository : Repository<OrderDetail>, IOrderDetailsRepository
    {
        private ApplicationDbContext _db;
        public OrderDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(OrderDetail obj)
        {
            _db.OrderDetails.Update(obj);
        }
    }
}
