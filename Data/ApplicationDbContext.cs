using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EShift.Models;


namespace EShift.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<Assistant> Assistants { get; set; }

        public DbSet<AssignedJob> AssignedJobs { get; set; }

        public DbSet<Admin> Admins { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CustomerOrder>()
                .HasKey(o => o.OrderId);

            // Configure the one-to-one relationship (optional, EF often infers this)
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Customer)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<Customer>(c => c.ApplicationUserId);
        }
    }
}