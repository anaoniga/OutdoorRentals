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
public class RentalItemsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RentalItemsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /api/RentalItemsApi/byRental/5
    [HttpGet("byRental/{rentalId:int}")]
    public async Task<ActionResult<IEnumerable<RentalItemDto>>> GetByRental(int rentalId)
    {
        var items = await _context.RentalItems
            .AsNoTracking()
            .Where(x => x.RentalId == rentalId)
            .OrderByDescending(x => x.Id)
            .Select(x => new RentalItemDto
            {
                Id = x.Id,
                RentalId = x.RentalId,
                EquipmentId = x.EquipmentId,
                Quantity = x.Quantity,
                DailyRate = x.PricePerDay   // ✅ MAPARE: DB PricePerDay -> API DailyRate
            })
            .ToListAsync();

        return Ok(items);
    }

    // POST: /api/RentalItemsApi
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] RentalItemDto dto)
    {
        if (dto.RentalId <= 0) return BadRequest("RentalId required.");
        if (dto.EquipmentId <= 0) return BadRequest("EquipmentId required.");
        if (dto.Quantity <= 0) return BadRequest("Quantity must be > 0.");
        if (dto.DailyRate < 0) return BadRequest("DailyRate must be >= 0.");

        var rentalExists = await _context.Rentals.AnyAsync(r => r.Id == dto.RentalId);
        if (!rentalExists) return BadRequest("Invalid RentalId.");

        var equipmentExists = await _context.Equipments.AnyAsync(e => e.Id == dto.EquipmentId);
        if (!equipmentExists) return BadRequest("Invalid EquipmentId.");

        var entity = new RentalItem
        {
            RentalId = dto.RentalId,
            EquipmentId = dto.EquipmentId,
            Quantity = dto.Quantity,
            PricePerDay = dto.DailyRate  // ✅ MAPARE: API DailyRate -> DB PricePerDay
        };

        _context.RentalItems.Add(entity);
        await _context.SaveChangesAsync();

        return Ok(new { id = entity.Id });
    }

    // PUT: /api/RentalItemsApi/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] RentalItemDto dto)
    {
        if (id != dto.Id) return BadRequest("Id mismatch.");
        if (dto.Quantity <= 0) return BadRequest("Quantity must be > 0.");
        if (dto.DailyRate < 0) return BadRequest("DailyRate must be >= 0.");

        var entity = await _context.RentalItems.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        entity.EquipmentId = dto.EquipmentId;
        entity.Quantity = dto.Quantity;
        entity.PricePerDay = dto.DailyRate; // ✅

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: /api/RentalItemsApi/5
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _context.RentalItems.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _context.RentalItems.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public class RentalItemDto
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public int EquipmentId { get; set; }
        public int Quantity { get; set; }
        public decimal DailyRate { get; set; } // ✅ API contract consistent cu Mobile
    }
}
