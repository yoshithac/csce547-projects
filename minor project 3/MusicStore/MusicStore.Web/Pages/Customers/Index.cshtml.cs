using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicStore.Data;

namespace MusicStore.Web.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly CustomerRepo _repo;
    public IndexModel(CustomerRepo repo) => _repo = repo;
    public List<Customer> Customers { get; set; } = new();

    public void OnGet() => Customers = _repo.GetAll();

    public IActionResult OnPostDelete(int id)
    {
        _repo.Delete(id);
        TempData["Message"] = "Customer deleted.";
        return RedirectToPage();
    }
}
