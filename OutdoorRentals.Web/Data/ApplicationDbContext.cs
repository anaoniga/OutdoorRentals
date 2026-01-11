using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OutdoorRentals.Web.Models;

namespace OutdoorRentals.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { 
        }
            public DbSet<EquipmentCategory> EquipmentCategories { get; set; } = default!;
            public DbSet<Equipment> Equipments { get; set; } = default!;
            public DbSet<Customer> Customers { get; set; } = default!;
            public DbSet<Rental> Rentals { get; set; } = default!;
            public DbSet<RentalItem> RentalItems { get; set; } = default!;
            public DbSet<Payment> Payments { get; set; } = default!;

        }
    }

