using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.DataAccess.Repository.IRepository; // UnitOfWork ke liye
using MVC.Models;

namespace MVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork; // 1. Field add ki

        // 2. Constructor mein IUnitOfWork inject kiya
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // 3. Database se products liye (Category ke saath)
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");


            // 4. Products ko View mein bhej diya
            return View(productList);
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
                return Json(new { data = new List<object>() });

            var products = _unitOfWork.Product.GetAll(includeProperties: "Category")
                .Where(p => p.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            p.Author.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Select(p => new {
                    id = p.Id,
                    title = p.Title,
                    author = p.Author,
                    price = p.Price100,
                    imageUrl = p.ImageUrl
                }).Take(5).ToList();

            return Json(new { data = products });
        }
        public IActionResult Details(int productId)
        {
            Product product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category");
            if (product == null)
            {
                return NotFound();
            }
            return PartialView("_ProductDetailsPartial", product); // Hum ek Partial View return karenge
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [Authorize] // Sirf login user product add kar sakega
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            // Login user ki ID nikalna
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            // Check karein ke kya ye book pehle se cart mein hai?
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId
                && u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                // Agar pehle se hai to quantity barha dein
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                // Naya entry add karein
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}