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
public class CustomersApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomersApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var items = await _context.Customers
            .AsNoTracking()
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                FullName = c.FullName
            })
            .ToListAsync();

        return Ok(items);
    }

    public class CustomerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
    }
}
