using HomeNet.Core.Modules.Persons.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeNet.Infrastructure.Persistence.Modules.Persons;

public sealed class PersonDbContext : DbContext
{
    public DbSet<Person> Persons => Set<Person>();

    public PersonDbContext(DbContextOptions<PersonDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("persons");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.AliasName)
                .HasColumnName("alias_name")
                .HasMaxLength(50);
            
            entity.Property(e => e.IsInactive)
                .HasColumnName("is_inactive")
                .HasDefaultValue(false);
        });

        modelBuilder.HasDefaultSchema("persons");
    }
}
