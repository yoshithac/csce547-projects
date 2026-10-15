using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicStore.Web.Pages;

public class SettingsModel : PageModel
{
    [BindProperty] public string Theme { get; set; } = "light";
    [BindProperty] public bool AdminMode { get; set; }

    public void OnGet()
    {
        Theme = Request.Cookies["theme"] == "dark" ? "dark" : "light";
        AdminMode = Request.IsAdmin();
    }

    public IActionResult OnPost()
    {
        var opt = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true };
        Response.Cookies.Append("theme", Theme == "dark" ? "dark" : "light", opt);
        Response.Cookies.Append("admin", AdminMode ? "1" : "0", opt);
        TempData["Message"] = "Settings saved.";
        return RedirectToPage();
    }
}
