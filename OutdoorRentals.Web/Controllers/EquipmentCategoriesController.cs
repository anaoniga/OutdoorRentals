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
    public class EquipmentCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipmentCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EquipmentCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.EquipmentCategories.ToListAsync());
        }

        // GET: EquipmentCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentCategory = await _context.EquipmentCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipmentCategory == null)
            {
                return NotFound();
            }

            return View(equipmentCategory);
        }

        // GET: EquipmentCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EquipmentCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] EquipmentCategory equipmentCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipmentCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equipmentCategory);
        }

        // GET: EquipmentCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentCategory = await _context.EquipmentCategories.FindAsync(id);
            if (equipmentCategory == null)
            {
                return NotFound();
            }
            return View(equipmentCategory);
        }

        // POST: EquipmentCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] EquipmentCategory equipmentCategory)
        {
            if (id != equipmentCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipmentCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentCategoryExists(equipmentCategory.Id))
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
            return View(equipmentCategory);
        }

        // GET: EquipmentCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentCategory = await _context.EquipmentCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipmentCategory == null)
            {
                return NotFound();
            }

            return View(equipmentCategory);
        }

        // POST: EquipmentCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipmentCategory = await _context.EquipmentCategories.FindAsync(id);
            if (equipmentCategory != null)
            {
                _context.EquipmentCategories.Remove(equipmentCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentCategoryExists(int id)
        {
            return _context.EquipmentCategories.Any(e => e.Id == id);
        }
    }
}
