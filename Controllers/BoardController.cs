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
        title = title?.Trim() ?? "";
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
        title = title?.Trim() ?? "";
        if (title.Length is 0 or > 200) return BadRequest();

        var card = await _db.Cards.FindAsync(id);
        if (card == null) return NotFound();

        card.Title = title;
        await _db.SaveChangesAsync();
        return Json(new { title = card.Title });
    }
}