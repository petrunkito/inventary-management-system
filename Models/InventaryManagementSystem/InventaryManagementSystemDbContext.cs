using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

public partial class InventaryManagementSystemDbContext : DbContext
{
    public InventaryManagementSystemDbContext()
    {
    }

    public InventaryManagementSystemDbContext(DbContextOptions<InventaryManagementSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<DepartureDetail> DepartureDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductInput> ProductInputs { get; set; }

    public virtual DbSet<ProductOutput> ProductOutputs { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<TypeInput> TypeInputs { get; set; }

    public virtual DbSet<TypeOutput> TypeOutputs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("User ID=postgres;Password=console.log();Server=localhost;Port=5432;Database=inventary_management_system;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("LOCALTIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customers_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("LOCALTIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<DepartureDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("departure_details_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Product).WithMany(p => p.DepartureDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_id");

            entity.HasOne(d => d.ProductOutput).WithMany(p => p.DepartureDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_output_id");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("LOCALTIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Stock).HasDefaultValue(0);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_category_id");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_supplier_id");
        });

        modelBuilder.Entity<ProductInput>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_inputs_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("LOCALTIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.InputType).WithMany(p => p.ProductInputs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_input_type_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductInputs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_id");
        });

        modelBuilder.Entity<ProductOutput>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_outputs_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Date).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Customer).WithMany(p => p.ProductOutputs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_customer_id");

            entity.HasOne(d => d.OutputType).WithMany(p => p.ProductOutputs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_output_type_id");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("suppliers_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("LOCALTIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TypeInput>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("type_inputs_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("LOCALTIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TypeOutput>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("type_outputs_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
