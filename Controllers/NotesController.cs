using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.DTOs;
using NotesApi.Models;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly NotesDbContext _context;

    public NotesController(NotesDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll()
    {
        var notes = await _context.Notes
            .OrderByDescending(note => note.CreatedAt)
            .Select(note => new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                IsArchived = note.IsArchived,
                CreatedAt = note.CreatedAt
            })
            .ToListAsync();

        return Ok(notes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NoteDto>> GetById(int id)
    {
        var note = await _context.Notes.FindAsync(id);

        if (note is null)
        {
            return NotFound();
        }

        return Ok(ToDto(note));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<NoteDto>>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query parameter is required.");
        }

        var trimmedQuery = query.Trim();

        var notes = await _context.Notes
            .Where(note => EF.Functions.ILike(note.Title, $"%{trimmedQuery}%"))
            .OrderByDescending(note => note.CreatedAt)
            .Select(note => new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                IsArchived = note.IsArchived,
                CreatedAt = note.CreatedAt
            })
            .ToListAsync();

        return Ok(notes);
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create(CreateNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
        {
            return BadRequest("Title and Content cannot be empty.");
        }

        var note = new Note
        {
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            IsArchived = dto.IsArchived
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        var result = ToDto(note);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
        {
            return BadRequest("Title and Content cannot be empty.");
        }

        var note = await _context.Notes.FindAsync(id);

        if (note is null)
        {
            return NotFound();
        }

        note.Title = dto.Title.Trim();
        note.Content = dto.Content.Trim();
        note.IsArchived = dto.IsArchived;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _context.Notes.FindAsync(id);

        if (note is null)
        {
            return NotFound();
        }

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static NoteDto ToDto(Note note)
    {
        return new NoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            IsArchived = note.IsArchived,
            CreatedAt = note.CreatedAt
        };
    }
}
