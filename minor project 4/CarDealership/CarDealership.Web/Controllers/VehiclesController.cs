using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarDealership.Data.Context;
using CarDealership.Data.Entities;

namespace CarDealership.Web.Controllers;

public class VehiclesController : Controller
{
    private readonly DealershipDbContext _context;

    public VehiclesController(DealershipDbContext context)
    {
        _context = context;
    }

    // GET: Vehicles (With Search on Make, Model, and VIN)
    public async Task<IActionResult> Index(string? searchString)
    {
        var query = _context.Vehicles.Include(v => v.Dealership).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            string term = searchString.Trim();
            query = query.Where(v => v.Make.Contains(term)
                                  || v.Model.Contains(term)
                                  || v.VIN.Contains(term));
        }

        ViewData["CurrentFilter"] = searchString;
        return View(await query.ToListAsync());
    }

    // GET: Vehicles/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vehicle = await _context.Vehicles
            .Include(v => v.Dealership)
            .FirstOrDefaultAsync(m => m.VehicleId == id);

        if (vehicle == null) return NotFound();
        return View(vehicle);
    }

    // GET: Vehicles/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.DealershipId = new SelectList(await _context.Dealerships.ToListAsync(), "DealershipId", "BranchName");
        return View();
    }

    // POST: Vehicles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {
        if (ModelState.IsValid)
        {
            _context.Add(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.DealershipId = new SelectList(await _context.Dealerships.ToListAsync(), "DealershipId", "BranchName", vehicle.DealershipId);
        return View(vehicle);
    }

    // GET: Vehicles/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return NotFound();

        ViewBag.DealershipId = new SelectList(await _context.Dealerships.ToListAsync(), "DealershipId", "BranchName", vehicle.DealershipId);
        return View(vehicle);
    }

    // POST: Vehicles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vehicle vehicle)
    {
        if (id != vehicle.VehicleId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Vehicles.Any(e => e.VehicleId == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.DealershipId = new SelectList(await _context.Dealerships.ToListAsync(), "DealershipId", "BranchName", vehicle.DealershipId);
        return View(vehicle);
    }

    // GET: Vehicles/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var vehicle = await _context.Vehicles
            .Include(v => v.Dealership)
            .FirstOrDefaultAsync(m => m.VehicleId == id);

        if (vehicle == null) return NotFound();
        return View(vehicle);
    }

    // POST: Vehicles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}