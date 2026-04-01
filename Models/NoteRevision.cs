using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models;

public class NoteRevision
{
    public int Id { get; set; }

    public int NoteId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public bool IsArchived { get; set; }

    [Required]
    [MaxLength(20)]
    public string Action { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; }
}
