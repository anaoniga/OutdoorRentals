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
public class RentalsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RentalsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RentalDto>>> GetAll()
    {
        var items = await _context.Rentals
            .AsNoTracking()
            .OrderByDescending(r => r.Id)
            .Select(r => new RentalDto
            {
                Id = r.Id,
                CustomerId = r.CustomerId,
                StartDate = r.StartDate,
                EndDate = r.EndDate
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] RentalDto dto)
    {
        if (dto.CustomerId <= 0) return BadRequest("CustomerId required.");
        if (dto.EndDate < dto.StartDate) return BadRequest("EndDate must be >= StartDate.");

        var customerExists = await _context.Customers.AnyAsync(c => c.Id == dto.CustomerId);
        if (!customerExists) return BadRequest("Invalid CustomerId.");

        var entity = new Rental
        {
            CustomerId = dto.CustomerId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

        _context.Rentals.Add(entity);
        await _context.SaveChangesAsync();

        return Ok(new { id = entity.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] RentalDto dto)
    {
        if (id != dto.Id) return BadRequest("Id mismatch.");
        if (dto.CustomerId <= 0) return BadRequest("CustomerId required.");
        if (dto.EndDate < dto.StartDate) return BadRequest("EndDate must be >= StartDate.");

        var entity = await _context.Rentals.FirstOrDefaultAsync(r => r.Id == id);
        if (entity == null) return NotFound();

        var customerExists = await _context.Customers.AnyAsync(c => c.Id == dto.CustomerId);
        if (!customerExists) return BadRequest("Invalid CustomerId.");

        entity.CustomerId = dto.CustomerId;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _context.Rentals.FirstOrDefaultAsync(r => r.Id == id);
        if (entity == null) return NotFound();

        // dacă ai cascade ok, merge direct; dacă nu, întâi ștergi RentalItems
        var items = await _context.RentalItems.Where(ri => ri.RentalId == id).ToListAsync();
        if (items.Count > 0) _context.RentalItems.RemoveRange(items);

        _context.Rentals.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public class RentalDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
