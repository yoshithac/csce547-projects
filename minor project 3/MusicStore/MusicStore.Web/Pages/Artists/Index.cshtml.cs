using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MusicStore.Data;

namespace MusicStore.Web.Pages.Artists;

public class IndexModel : PageModel
{
    private readonly ArtistRepo _repo;
    public IndexModel(ArtistRepo repo) => _repo = repo;
    public List<Artist> Artists { get; set; } = new();
    public bool IsAdmin => Request.IsAdmin();

    public void OnGet() => Artists = _repo.GetAll();

    public IActionResult OnPostDelete(int id)
    {
        if (!Request.IsAdmin()) { TempData["Error"] = "Admin Mode is required to change artists. Turn it on in Settings."; return RedirectToPage(); }
        try { _repo.Delete(id); TempData["Message"] = "Artist deleted."; }
        catch (SqlException ex) when (ex.Number == 547)
        { TempData["Error"] = "That artist still has albums. Delete or reassign those albums first."; }
        return RedirectToPage();
    }
}
