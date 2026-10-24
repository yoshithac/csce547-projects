using Microsoft.AspNetCore.Mvc;

namespace CarDealership.Web.Controllers;

public class SettingsController : Controller
{
    public IActionResult Index()
    {
        ViewBag.Theme = Request.Cookies["AppTheme"] ?? "light";
        ViewBag.Density = Request.Cookies["AppDensity"] ?? "standard";
        return View();
    }

    [HttpPost]
    public IActionResult SaveSettings(string theme, string density)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true
        };

        Response.Cookies.Append("AppTheme", theme, cookieOptions);
        Response.Cookies.Append("AppDensity", density, cookieOptions);

        TempData["SuccessMessage"] = "Preferences saved successfully!";
        return RedirectToAction(nameof(Index));
    }
}