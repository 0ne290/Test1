using System.Diagnostics;
using Core.Application;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class HomeController(ILogger<HomeController> logger, DrinksVendingMachineInteractor vending)
    : Controller
{
    public IActionResult Index()
    {
        ViewData["Drinks"] = vending.GetAllDrinks();
        return View();
    }

    public ContentResult DepositeCoin(int denomination) => Content(vending.DepositeCoin(denomination).ToString());

    public async Task<ContentResult> ChooseDrink(int drinkKey) => Content((await vending.ChooseDrink(drinkKey)).ToString());
    
    public JsonResult BuyDrinks() => Json(vending.BuyDrinks());
    
    public OkResult ResetSelection()
    {
        vending.ResetSelection();

        return Ok();
    }

    public ContentResult GetRest() => Content(vending.Rest.ToString());
    
    public ContentResult GetDeposite() => Content(vending.DepositedAmount.ToString());

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    private readonly ILogger<HomeController> _logger = logger;
}
