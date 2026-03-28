using HomeNet.Infrastructure.Persistence.Modules.Auth.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Cards.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Persons.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeNet.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AddPersonModule(modelBuilder);
        AddAuthModule(modelBuilder);
        AddCardModule(modelBuilder);
    }

    private void AddPersonModule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PersonEntity>()
            .HasKey(e => e.Id);
    }

    private void AddAuthModule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<UserEntity>()
            .HasOne(e => e.Person)
            .WithOne()
            .HasForeignKey<UserEntity>(e => e.PersonId);
    }

    private void AddCardModule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardEntity>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<CardEntity>()
            .HasOne(e => e.Person)
            .WithMany()
            .HasForeignKey(e => e.PersonId);
    }
}
