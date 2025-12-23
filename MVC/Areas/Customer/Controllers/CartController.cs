using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.DataAccess.Repository.IRepository;
using MVC.Models;
using MVC.Models.ViewModels;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCartVM ShoppingCartVM { get; set; }

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        // Cart load karte waqt Product include lazmi karein
        var cartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

        ShoppingCartVM = new()
        {
            ShoppingCartList = cartList,
            OrderHeader = new()
        };

        // User details fetch karein
        var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

        // 500 Error se bachne ke liye Null Check aur calculation zaroori hai
        if (user != null)
        {
            //ShoppingCartVM.OrderHeader.Name = user.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            //ShoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            //ShoppingCartVM.OrderHeader.City = user.City;
        }

        // View mein prices dikhane ke liye calculation lazmi hai
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
        shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, "Product");

        // 2. OrderHeader ki basic details set karein
        shoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
        shoppingCartVM.OrderHeader.ApplicationUserId = userId;

        foreach (var cart in shoppingCartVM.ShoppingCartList)
        {
            shoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
        }

        // 3. Order Save karein
        _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
        _unitOfWork.Save();

        // 4. Order hone ke baad Cart khali kar dein
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCartVM.ShoppingCartList);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index), "Home");
    }
}