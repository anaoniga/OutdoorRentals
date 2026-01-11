using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutdoorRentals.Web.Data;
using OutdoorRentals.Web.Models;

namespace OutdoorRentals.Web.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

public class EquipmentCategoriesApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public EquipmentCategoriesApiController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquipmentCategory>>> GetAll()
        => await _db.EquipmentCategories.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<EquipmentCategory>> GetById(int id)
    {
        var item = await _db.EquipmentCategories.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<EquipmentCategory>> Create(EquipmentCategory model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _db.EquipmentCategories.Add(model);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, EquipmentCategory model)
    {
        if (id != model.Id) return BadRequest("ID mismatch");

        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.EquipmentCategories.FindAsync(id);
        if (item == null) return NotFound();

        _db.EquipmentCategories.Remove(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

