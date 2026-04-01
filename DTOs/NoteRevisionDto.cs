namespace NotesApi.DTOs;

public class NoteRevisionDto
{
    public int Id { get; set; }
    public int NoteId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
}
