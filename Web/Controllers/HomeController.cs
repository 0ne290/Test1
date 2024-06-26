﻿using System.Diagnostics;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class HomeController(ILogger<HomeController> logger, DrinksVendingMachine vending)
    : Controller
{
    public IActionResult Index()
    {
        ViewData["Drinks"] = vending.GetAllDrinks();
        ViewData["Coins"] = vending.GetAllCoins();
        return View();
    }

    // Если все-таки юзать JWT, то этот эндпоинт должен использоваться для получения токена
    //public ContentResult GetAdminToken(string login, string password)
    //{
    //    const string secretLogin = "93555EA12CFBFB63CEAC169A3D59044B3A81A96C0BF636A5892B05557FDF399F";
    //    const string secretPassword = "F2966F4292F082BD794F69D3617F5E10C40C35DED7DA48AB7ACCE5AE98F99C5A";
    //    
    //    if (login.GetHashString() != secretLogin || password.GetHashString() != secretPassword)
    //        return Content(string.Empty);
    //    
    //    var jwt = new JwtSecurityToken(
    //        issuer: "TestServer",
    //        audience: "TestServerClient",
    //        claims: new[] { new Claim(ClaimsIdentity.DefaultRoleClaimType, "Administrator") },
    //        signingCredentials: new SigningCredentials(
    //            new SymmetricSecurityKey("mysupersecret_secretsecretsecretkey!123"u8.ToArray()),
    //            SecurityAlgorithms.HmacSha256));
    //    
    //    return Content(new JwtSecurityTokenHandler().WriteToken(jwt));
    //}

    public OkResult DepositeCoin(int denomination)
    {
        vending.DepositeCoin(denomination);

        return Ok();
    }

    public async Task<ContentResult> ChooseDrink(int drinkKey) => Content((await vending.ChooseDrink(drinkKey)).ToString());
    
    public async Task<JsonResult> BuyDrinks() => Json(await vending.BuyDrinks());

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
