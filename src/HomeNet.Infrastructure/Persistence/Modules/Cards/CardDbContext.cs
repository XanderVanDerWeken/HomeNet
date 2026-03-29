using System;
using HomeNet.Core.Modules.Cards.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeNet.Infrastructure.Persistence.Modules.Cards;

public sealed class CardDbContext : DbContext
{
    public DbSet<Card> Cards => Set<Card>();

    public CardDbContext(DbContextOptions<CardDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(entity =>
        {
            entity.ToTable("cards");
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.ExpirationDate)
                .HasColumnName("expiration_date")
                .IsRequired();
            
            entity.Property(e => e.PersonId)
                .HasColumnName("person_id")
                .IsRequired();
        });

        modelBuilder.HasDefaultSchema("cards");
    }
}
