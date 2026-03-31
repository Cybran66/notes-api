using Microsoft.EntityFrameworkCore;
using NotesApi.Models;

namespace NotesApi.Data;

public class NotesDbContext : DbContext
{
    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    {
    }

    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>(entity =>
        {
            entity.Property(note => note.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(note => note.Content)
                .IsRequired();

            entity.Property(note => note.CreatedAt)
                .IsRequired();
        });
    }

    public override int SaveChanges()
    {
        SetCreatedAtForNewNotes();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetCreatedAtForNewNotes();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetCreatedAtForNewNotes()
    {
        var newNotes = ChangeTracker.Entries<Note>()
            .Where(entry => entry.State == EntityState.Added);

        foreach (var entry in newNotes)
        {
            if (entry.Entity.CreatedAt == default)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}
