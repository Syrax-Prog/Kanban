using kanban.Data;
using kanban.Models;
namespace kanban.Data;

public static class DbSeeder
{
    public static void Seed(KanbanDbContext db)
    {
        if(db.Boards.Any()) return; //dont seed twice

        var board = new Board
        {
            Name = "my First Board",
            Columns = new List<Column>
            {
                new() { Name = "To Do", Position = 1 },
                new() { Name = "Doing", Position = 2 },
                new() { Name = "Done", Position = 3 }
            }
        };

        db.Boards.Add(board);
        db.SaveChanges();
    }
}