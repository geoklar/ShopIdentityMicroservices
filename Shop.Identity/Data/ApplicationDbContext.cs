using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop.Common.Models;
using Shop.Identity.Dtos;

namespace Shop.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<Shop.Common.Models.ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // public DbSet<ApplicationUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<ApplicationUser>(entity =>
    //    {

    //        entity.ToTable("User");
    //        entity.HasKey(p => p.Id).HasName("PK_User");
    //        entity.Property(p => p.Id)
    //        .HasColumnName("id")
    //        .HasColumnType("int").ValueGeneratedNever();
    //        entity.Property(p => p.Name)
    //        .HasColumnName("name");
    //    });

    //}

    // public DbSet<ApplicationUser> Users { get; set; }
}
