using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OutdoorRentals.Web.Data;
using OutdoorRentals.Web.Models;

namespace OutdoorRentals.Web.Controllers
{
    public class RentalItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RentalItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RentalItems.Include(r => r.Equipment).Include(r => r.Rental);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentalItem = await _context.RentalItems
                .Include(r => r.Equipment)
                .Include(r => r.Rental)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rentalItem == null)
            {
                return NotFound();
            }

            return View(rentalItem);
        }

        public IActionResult Create()
        {
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Name");
            ViewData["RentalId"] = new SelectList(_context.Rentals, "Id", "Status");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Quantity,PricePerDay,RentalId,EquipmentId")] RentalItem rentalItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rentalItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Name", rentalItem.EquipmentId);
            ViewData["RentalId"] = new SelectList(_context.Rentals, "Id", "Status", rentalItem.RentalId);
            return View(rentalItem);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentalItem = await _context.RentalItems.FindAsync(id);
            if (rentalItem == null)
            {
                return NotFound();
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Name", rentalItem.EquipmentId);
            ViewData["RentalId"] = new SelectList(_context.Rentals, "Id", "Status", rentalItem.RentalId);
            return View(rentalItem);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantity,PricePerDay,RentalId,EquipmentId")] RentalItem rentalItem)
        {
            if (id != rentalItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rentalItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalItemExists(rentalItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Name", rentalItem.EquipmentId);
            ViewData["RentalId"] = new SelectList(_context.Rentals, "Id", "Status", rentalItem.RentalId);
            return View(rentalItem);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentalItem = await _context.RentalItems
                .Include(r => r.Equipment)
                .Include(r => r.Rental)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rentalItem == null)
            {
                return NotFound();
            }

            return View(rentalItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rentalItem = await _context.RentalItems.FindAsync(id);
            if (rentalItem != null)
            {
                _context.RentalItems.Remove(rentalItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalItemExists(int id)
        {
            return _context.RentalItems.Any(e => e.Id == id);
        }
    }
}
