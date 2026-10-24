using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarDealership.Data.Context;
using CarDealership.Data.Entities;

namespace CarDealership.Web.Controllers;

public class AdminController : Controller
{
    private readonly DealershipDbContext _context;

    public AdminController(DealershipDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Dealerships
    public async Task<IActionResult> Index()
    {
        return View(await _context.Dealerships.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var dealership = await _context.Dealerships
            .Include(d => d.Vehicles)
            .Include(d => d.Salespeople)
            .FirstOrDefaultAsync(m => m.DealershipId == id);

        if (dealership == null) return NotFound();
        return View(dealership);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Dealership dealership)
    {
        if (ModelState.IsValid)
        {
            _context.Add(dealership);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(dealership);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var dealership = await _context.Dealerships.FindAsync(id);
        if (dealership == null) return NotFound();
        return View(dealership);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Dealership dealership)
    {
        if (id != dealership.DealershipId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(dealership);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Dealerships.Any(e => e.DealershipId == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(dealership);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var dealership = await _context.Dealerships.FirstOrDefaultAsync(m => m.DealershipId == id);
        if (dealership == null) return NotFound();
        return View(dealership);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var dealership = await _context.Dealerships.FindAsync(id);
        if (dealership != null)
        {
            _context.Dealerships.Remove(dealership);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}