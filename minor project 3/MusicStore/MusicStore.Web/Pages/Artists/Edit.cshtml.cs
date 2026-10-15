using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicStore.Data;

namespace MusicStore.Web.Pages.Artists;

public class EditModel : PageModel
{
    private readonly ArtistRepo _repo;
    public EditModel(ArtistRepo repo) => _repo = repo;

    [BindProperty] public Artist Item { get; set; } = new();

    private IActionResult NotAdmin()
    {
        TempData["Error"] = "Admin Mode is required to change artists. Turn it on in Settings.";
        return RedirectToPage("Index");
    }

    public IActionResult OnGet(int? id)
    {
        if (!Request.IsAdmin()) return NotAdmin();
        if (id.HasValue)
        {
            Item = _repo.Get(id.Value);
            if (Item == null) return NotFound();
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!Request.IsAdmin()) return NotAdmin();
        if (!ModelState.IsValid) return Page();
        if (Item.ArtistId == 0) { _repo.Add(Item); TempData["Message"] = "Artist added."; }
        else { _repo.Update(Item); TempData["Message"] = "Artist updated."; }
        return RedirectToPage("Index");
    }
}
