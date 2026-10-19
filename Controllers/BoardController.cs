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

        if(board == null) return NotFound("No Board Found In The Database");

        return View(board);
    }
}