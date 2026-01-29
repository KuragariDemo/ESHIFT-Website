using EShift.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EShift.Data;


public class EShiftContext : IdentityDbContext<ApplicationUser>

{
    public EShiftContext(DbContextOptions<EShiftContext> options)
        : base(options)
    {
    }

    public DbSet<CustomerOrder> CustomerOrders { get; set; }

    public DbSet<Customer> Customers { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
