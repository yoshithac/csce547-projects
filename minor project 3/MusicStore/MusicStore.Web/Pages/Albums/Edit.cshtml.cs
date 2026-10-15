using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStore.Data;

namespace MusicStore.Web.Pages.Albums;

public class EditModel : PageModel
{
    private readonly AlbumRepo _albums;
    private readonly ArtistRepo _artists;
    public EditModel(AlbumRepo albums, ArtistRepo artists) { _albums = albums; _artists = artists; }

    [BindProperty] public Album Item { get; set; } = new();
    public List<SelectListItem> ArtistOptions { get; set; } = new();

    public IActionResult OnGet(int? id)
    {
        if (id.HasValue)
        {
            Item = _albums.Get(id.Value);
            if (Item == null) return NotFound();
        }
        LoadLists();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) { LoadLists(); return Page(); }
        if (Item.AlbumId == 0) { _albums.Add(Item); TempData["Message"] = "Album added."; }
        else { _albums.Update(Item); TempData["Message"] = "Album updated."; }
        return RedirectToPage("Index");
    }

    private void LoadLists() =>
        ArtistOptions = _artists.GetAll().Select(a => new SelectListItem(a.Name, a.ArtistId.ToString())).ToList();
}
