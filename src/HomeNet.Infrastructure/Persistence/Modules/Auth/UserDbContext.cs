using HomeNet.Core.Modules.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeNet.Infrastructure.Persistence.Modules.Auth;

public sealed class UserDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public UserDbContext(DbContextOptions<UserDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserName)
                .HasColumnName("user_name")
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Role)
                .HasColumnName("role")
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(e => e.PersonId)
                .HasColumnName("person_id");

            entity.HasIndex(e => e.UserName)
                .IsUnique();
        });

        modelBuilder.HasDefaultSchema("users");
    }
}
