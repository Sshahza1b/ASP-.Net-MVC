using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.DataAccess.Repository.IRepository;
using MVC.Models;
using MVC.Models.ViewModels;

[Area("Customer")]
//[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Is Method ki wajah se 404 aa raha tha, ab ye sahi hai
    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
            OrderHeader = new()
        };

        if (ShoppingCartVM.ShoppingCartList != null)
        {
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                // Price calculation logic
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
            }
        }

        return View(ShoppingCartVM);
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
            OrderHeader = new()
        };

        // User details fill karna (Jo aapne manga tha)
        var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        if (user != null)
        {
            //ShoppingCartVM.OrderHeader.Name = user.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            //ShoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            //ShoppingCartVM.OrderHeader.City = user.City;
        }

        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
        }

        return View(ShoppingCartVM);
    }

    [HttpPost]
    [ActionName("Summary")]
    public IActionResult SummaryPOST(ShoppingCartVM shoppingCartVM)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        // 1. Cart Items load karein
        shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

        // CHECK 1: Agar cart khali hai
        if (shoppingCartVM.ShoppingCartList.Count() == 0)
        {
            TempData["error"] = "Aapka cart khali hai! Please select items first.";
            return RedirectToAction("Index", "Home");
        }

        // CHECK 2: Agar details null hain
        if (string.IsNullOrEmpty(shoppingCartVM.OrderHeader.Name) ||
            string.IsNullOrEmpty(shoppingCartVM.OrderHeader.PhoneNumber))
        {
            TempData["error"] = "Please fill all details before confirming.";
            return RedirectToAction(nameof(Summary));
        }

        // 2. OrderHeader details set karein
        shoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
        shoppingCartVM.OrderHeader.ApplicationUserId = userId;

        foreach (var cart in shoppingCartVM.ShoppingCartList)
        {
            shoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
        }

        // 3. Pehle OrderHeader (Main Order) save karein
        _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
        _unitOfWork.Save();

        // 4. AB HAR PRODUCT KI DETAIL SAVE KAREIN (Ye zaroori hai)
        foreach (var cart in shoppingCartVM.ShoppingCartList)
        {
            OrderDetail orderDetail = new()
            {
                ProductId = cart.ProductId,
                OrderHeaderId = shoppingCartVM.OrderHeader.Id, // Save hone ke baad ID auto-generate hoti hai
                Price = cart.Product.Price,
                Count = cart.Count
            };
            _unitOfWork.OrderDetails.Add(orderDetail);
            _unitOfWork.Save();
        }

        // 5. Cart khali kar dein
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCartVM.ShoppingCartList);
        _unitOfWork.Save();

        // 6. Redirect to Confirmation (Home ki jagah Success page behtar hai)
        return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id });
    }

    public IActionResult OrderConfirmation(int id)
    {
        return View(id); // Order ID view ko pass kar rahe hain
    }

    public IActionResult Plus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        cartFromDb.Count += 1;
        _unitOfWork.ShoppingCart.Update(cartFromDb);
        _unitOfWork.Save();

        // Yahan Index ki jagah Summary par wapis bhejein agar user Summary se aaya hai
        return RedirectToAction(nameof(Summary));
    }

    public IActionResult Minus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        if (cartFromDb.Count <= 1)
        {
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
        }
        else
        {
            cartFromDb.Count -= 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }
        _unitOfWork.Save();

        return RedirectToAction(nameof(Summary));
    }

    public IActionResult Remove(int cartId)
    {
        // Database se wo specific cart item uthayein
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

        // Item ko remove karein
        _unitOfWork.ShoppingCart.Remove(cartFromDb);
        _unitOfWork.Save();

        // Wapis Summary page par bhej dein taake user ko updated list dikhe
        return RedirectToAction(nameof(Summary));
    }

}