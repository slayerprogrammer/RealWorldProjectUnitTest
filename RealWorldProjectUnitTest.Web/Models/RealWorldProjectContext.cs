using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RealWorldProjectUnitTest.Web.Models;

public partial class RealWorldProjectContext : DbContext
{
    public RealWorldProjectContext()
    {
    }

    public RealWorldProjectContext(DbContextOptions<RealWorldProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Category> Category { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=RealWorldProject;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        //uygulama ilk ayağa kalktığında migration işleminden sonra fake data oluşturmak için 
        //fake data dbseed işlemi gerçekleştiyoruz.
        //modelBuilder.Entity<Category>().HasData(new Category { Id = 1, Name = "Kalemler" }, new Category() { Id = 2, Name = "Defterler" });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
