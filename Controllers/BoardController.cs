using kanban.Data;
using kanban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kanban.Controllers;

public class BoardController : Controller
{
    private readonly KanbanDbContext _db;
    public BoardController(KanbanDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var board = await _db.Boards
            .Include(b => b.Columns.OrderBy(c => c.Position))
                .ThenInclude(c => c.Cards.OrderBy(card => card.Position))
            .FirstOrDefaultAsync();

        if (board == null) return NotFound("No Board Found In The Database");

        return View(board);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RenameBoard(int id, string? title)
    {
        title = title?.Trim() ?? "Untitled";
        if (title.Length is 0 or > 100) return BadRequest();

        var board = await _db.Boards.FindAsync(id);
        if (board == null) return NotFound();

        board.Name = title;
        await _db.SaveChangesAsync();
        return Json(new { title = board.Name });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RenameCard(int id, string? title)
    {
        title = title?.Trim() ?? "Untitled";
        if (title.Length is 0 or > 200) return BadRequest();

        var card = await _db.Cards.FindAsync(id);
        if (card == null) return NotFound();

        card.Title = title;
        await _db.SaveChangesAsync();
        return Json(new { title = card.Title });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCard(int columnId, string? title)
    {
        title = title?.Trim() ?? "";
        if(title.Length <= 0) title = "Untitled";
        if(title.Length > 200) title = title[..200];

        var column = await _db.Columns.FindAsync(columnId);
        if(column == null) return NotFound();

        var lastPosition = await _db.Cards
            .Where(c => c.ColumnId == columnId)
            .MaxAsync(c => (int?)c.Position) ?? 0;

        _db.Cards.Add(new Card
        {
            Title = title,
            Description = "Description",
            ColumnId = columnId,
            Position = lastPosition + 1,
            CreateAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}