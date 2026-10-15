using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicStore.Data;

namespace MusicStore.Web.Pages.Customers;

public class EditModel : PageModel
{
    private readonly CustomerRepo _repo;
    public EditModel(CustomerRepo repo) => _repo = repo;

    [BindProperty] public Customer Item { get; set; } = new();

    public IActionResult OnGet(int? id)
    {
        if (id.HasValue)
        {
            Item = _repo.Get(id.Value);
            if (Item == null) return NotFound();
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        if (Item.CustomerId == 0) { _repo.Add(Item); TempData["Message"] = "Customer added."; }
        else { _repo.Update(Item); TempData["Message"] = "Customer updated."; }
        return RedirectToPage("Index");
    }
}
