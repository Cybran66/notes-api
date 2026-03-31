using System.ComponentModel.DataAnnotations;

namespace NotesApi.DTOs;

public class UpdateNoteDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public bool IsArchived { get; set; }
}
