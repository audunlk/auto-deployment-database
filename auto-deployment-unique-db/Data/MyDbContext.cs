using Microsoft.EntityFrameworkCore;
using auto_deployment_unique_db.Models;
using System;


namespace auto_deployment_unique_db
{
    public class MyDbContext : DbContext
    {
        private readonly string? _connectionString;

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public MyDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString != null)
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");
                entity.HasKey(c => c.Id).HasName("customer_id");
                entity.Property(c => c.Id).HasColumnName("id");
                entity.Property(c => c.FirstName).HasMaxLength(50).IsRequired().HasColumnName("first_name");
                entity.Property(c => c.LastName).HasMaxLength(50).IsRequired().HasColumnName("last_name");
                entity.HasData(
                    new Customer { Id = 1, FirstName = "John", LastName = "Smith" },
                    new Customer { Id = 2, FirstName = "Jane", LastName = "Doe" }
                );
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(p => p.Id).HasName("product_id");
                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.Name).HasMaxLength(100).IsRequired().HasColumnName("name");
                entity.Property(p => p.Price).IsRequired().HasColumnName("price");
                entity.HasData(
                    new Product { Id = 1, Name = "Product 1", Price = 10.99m },
                    new Product { Id = 2, Name = "Product 2", Price = 19.99m },
                    new Product { Id = 3, Name = "Product 3", Price = 7.99m }
                );
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.HasKey(o => o.Id).HasName("order_id");
                entity.Property(o => o.Id).HasColumnName("id");
                entity.Property(o => o.OrderDate).IsRequired().HasColumnName("order_date");
                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(o => o.CustomerId).HasColumnName("customer_id");
                entity.HasData(
                    new Order { Id = 1, OrderDate = DateTime.UtcNow, CustomerId = 1 },
                    new Order { Id = 2, OrderDate = DateTime.UtcNow, CustomerId = 1 },
                    new Order { Id = 3, OrderDate = DateTime.UtcNow, CustomerId = 2 }
                );
            });



        }
    }
}
