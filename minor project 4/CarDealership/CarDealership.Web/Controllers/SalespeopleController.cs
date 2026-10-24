using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarDealership.Data.Context;
using CarDealership.Data.Entities;

namespace CarDealership.Web.Controllers;

public class SalespeopleController : Controller
{
    private readonly DealershipDbContext _context;

    public SalespeopleController(DealershipDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Salespeople.Include(s => s.Dealership).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var person = await _context.Salespeople
            .Include(s => s.Dealership)
            .FirstOrDefaultAsync(m => m.SalespersonId == id);

        if (person == null) return NotFound();
        return View(person);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.DealershipId = new SelectList(await _context.Dealerships.ToListAsync(), "DealershipId", "BranchName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Salesperson salesperson)
    {
        if (ModelState.IsValid)
        {
            _context.Add(salesperson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.DealershipId = new SelectList(await _context.Dealerships.ToListAsync(), "DealershipId", "BranchName", salesperson.DealershipId);
        return View(salesperson);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var salesperson = await _context.Salespeople.FindAsync(id);
        if (salesperson == null) return NotFound();

        ViewBag.DealershipId = new SelectList(await _context.Dealerships.ToListAsync(), "DealershipId", "BranchName", salesperson.DealershipId);
        return View(salesperson);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Salesperson salesperson)
    {
        if (id != salesperson.SalespersonId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(salesperson);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Salespeople.Any(e => e.SalespersonId == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.DealershipId = new SelectList(await _context.Dealerships.ToListAsync(), "DealershipId", "BranchName", salesperson.DealershipId);
        return View(salesperson);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var salesperson = await _context.Salespeople
            .Include(s => s.Dealership)
            .FirstOrDefaultAsync(m => m.SalespersonId == id);

        if (salesperson == null) return NotFound();
        return View(salesperson);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var salesperson = await _context.Salespeople.FindAsync(id);
        if (salesperson != null)
        {
            _context.Salespeople.Remove(salesperson);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}