using System.Diagnostics;
using Core.Application;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class HomeController : Controller
{
    public HomeController(ILogger<HomeController> logger, DrinksVendingMachineInteractor vending)
    {
        _logger = logger;
        _vending = vending;
    }

    public IActionResult Index()
    {
        ViewData["Drinks"] = _vending.GetAllDrinks();
        return View();
    }

    public ContentResult GetRest() => Content(_vending.Rest.ToString());
    
    public ContentResult GetDeposite() => Content(_vending.DepositedAmount.ToString());

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    private readonly ILogger<HomeController> _logger;

    private readonly DrinksVendingMachineInteractor _vending;
}