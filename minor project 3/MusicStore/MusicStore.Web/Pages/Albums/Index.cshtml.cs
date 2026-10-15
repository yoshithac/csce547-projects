using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicStore.Data;

namespace MusicStore.Web.Pages.Albums;

public class IndexModel : PageModel
{
    private readonly AlbumRepo _repo;
    public IndexModel(AlbumRepo repo) => _repo = repo;

    [BindProperty(SupportsGet = true)] public string Q { get; set; }
    public List<Album> Albums { get; set; } = new();

    public void OnGet() => Albums = _repo.Search(Q);

    public IActionResult OnPostDelete(int id)
    {
        _repo.Delete(id);
        TempData["Message"] = "Album deleted.";
        return RedirectToPage(new { Q });
    }
}
