using System.ComponentModel.DataAnnotations;

namespace NotesApi.DTOs;

public class UpdateNoteDto
{
    [Required(ErrorMessage = "Поле Title обязательно.")]
    [MaxLength(100, ErrorMessage = "Title не должен превышать 100 символов.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле Content обязательно.")]
    public string Content { get; set; } = string.Empty;

    public bool IsArchived { get; set; }
}
