using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutdoorRentals.Web.Data;
using OutdoorRentals.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace OutdoorRentals.Web.Api; 

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

public class EquipmentsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EquipmentsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquipmentDto>>> GetAll()
    {
        var items = await _context.Equipments
            .AsNoTracking()
            .Select(e => new EquipmentDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                DailyRate = e.DailyRate,
                StockTotal = e.StockTotal,
                StockAvailable = e.StockAvailable,
                EquipmentCategoryId = e.EquipmentCategoryId
            })
            .ToListAsync();

        return Ok(items);
    }

    
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] EquipmentDto dto)
    {
        
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        if (dto.DailyRate < 0)
            return BadRequest("DailyRate must be >= 0.");

        if (dto.StockTotal < 0 || dto.StockAvailable < 0)
            return BadRequest("Stock values must be >= 0.");

        if (dto.StockAvailable > dto.StockTotal)
            return BadRequest("StockAvailable cannot be greater than StockTotal.");

        
        var catExists = await _context.EquipmentCategories.AnyAsync(c => c.Id == dto.EquipmentCategoryId);
        if (!catExists)
            return BadRequest("Invalid EquipmentCategoryId.");

        var entity = new Equipment
        {
            Name = dto.Name.Trim(),
            Description = dto.Description,
            DailyRate = dto.DailyRate,
            StockTotal = dto.StockTotal,
            StockAvailable = dto.StockAvailable,
            EquipmentCategoryId = dto.EquipmentCategoryId
        };

        _context.Equipments.Add(entity);
        await _context.SaveChangesAsync();

        return Ok(new { id = entity.Id });
    }

    
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] EquipmentDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id mismatch.");

        var entity = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        if (dto.DailyRate < 0)
            return BadRequest("DailyRate must be >= 0.");

        if (dto.StockTotal < 0 || dto.StockAvailable < 0)
            return BadRequest("Stock values must be >= 0.");

        if (dto.StockAvailable > dto.StockTotal)
            return BadRequest("StockAvailable cannot be greater than StockTotal.");

        var catExists = await _context.EquipmentCategories.AnyAsync(c => c.Id == dto.EquipmentCategoryId);
        if (!catExists)
            return BadRequest("Invalid EquipmentCategoryId.");

        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description;
        entity.DailyRate = dto.DailyRate;
        entity.StockTotal = dto.StockTotal;
        entity.StockAvailable = dto.StockAvailable;
        entity.EquipmentCategoryId = dto.EquipmentCategoryId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null)
            return NotFound();

        _context.Equipments.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    public class EquipmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public decimal DailyRate { get; set; }
        public int StockTotal { get; set; }
        public int StockAvailable { get; set; }
        public int EquipmentCategoryId { get; set; }
    }
}
