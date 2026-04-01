using Microsoft.EntityFrameworkCore;
using NotesApi.Models;

namespace NotesApi.Data;

public class NotesDbContext : DbContext
{
    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    {
    }

    public DbSet<Note> Notes => Set<Note>();
    public DbSet<NoteRevision> NoteRevisions => Set<NoteRevision>();

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

        modelBuilder.Entity<NoteRevision>(entity =>
        {
            entity.Property(revision => revision.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(revision => revision.Content)
                .IsRequired();

            entity.Property(revision => revision.Action)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(revision => revision.ChangedAt)
                .IsRequired();

            entity.HasIndex(revision => new { revision.NoteId, revision.ChangedAt });
        });
    }

    public override int SaveChanges()
    {
        SetCreatedAtForNewNotes();
        SetChangedAtForNewRevisions();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetCreatedAtForNewNotes();
        SetChangedAtForNewRevisions();
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

    private void SetChangedAtForNewRevisions()
    {
        var newRevisions = ChangeTracker.Entries<NoteRevision>()
            .Where(entry => entry.State == EntityState.Added);

        foreach (var entry in newRevisions)
        {
            if (entry.Entity.ChangedAt == default)
            {
                entry.Entity.ChangedAt = DateTime.UtcNow;
            }
        }
    }
}
