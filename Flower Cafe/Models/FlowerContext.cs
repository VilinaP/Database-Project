using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace Flower_Cafe.Models;

public partial class FlowerContext : DbContext
{
    public FlowerContext()
    {
    }

    public FlowerContext(DbContextOptions<FlowerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employees> Employees { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<MenuItemType> MenuItemTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WageRate> WageRates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("pk_Customer_id");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId).ValueGeneratedNever();
            entity.Property(e => e.City)
                .HasMaxLength(170)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(2)
                .IsUnicode(false);
            entity.Property(e => e.Street)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Zip).HasColumnName("ZIP");
        });

        modelBuilder.Entity<Employees>(entity =>
        {
            entity.HasKey(e => e.EmployeesId).HasName("pk_Employees_id");

            entity.Property(e => e.EmployeesId).ValueGeneratedNever();
            entity.Property(e => e.City)
                .HasMaxLength(170)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ssn)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("SSN");
            entity.Property(e => e.State)
                .HasMaxLength(2)
                .IsUnicode(false);
            entity.Property(e => e.Street)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Zip).HasColumnName("ZIP");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuItemId).HasName("pk_MenuItems_id");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.Type)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_MenuItems_Type");
        });

        modelBuilder.Entity<MenuItemType>(entity =>
        {
            entity.HasKey(e => e.MenuItemTypeid).HasName("pk_MenuItemType_id");

            entity.ToTable("MenuItemType");

            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("pk_Orders_id");

            entity.Property(e => e.SalesTax).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(8, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Orders_CustomerId");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.MenuItemId }).HasName("pk_OrderItems_id");

            entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.MenuItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_OrderItems_MenuItemId");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_OrderItems_OrderId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username, "UQ_Username").IsUnique();

            entity.Property(e => e.UserType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WageRate>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeId, e.StartDate }).HasName("pk_WageRate_id");

            entity.ToTable("WageRate");

            entity.Property(e => e.Wage).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.WageType)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.WageRates)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_WageRate_EmployeeId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
