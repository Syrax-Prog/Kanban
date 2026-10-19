using kanban.Models;
using Microsoft.EntityFrameworkCore;

namespace kanban.Data;

public class KanbanDbContext : DbContext
{
    public KanbanDbContext(DbContextOptions<KanbanDbContext> options) : base(options){}

    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Column> Columns => Set<Column>();
    public DbSet<Card> Cards => Set<Card>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(e =>
        {
           e.Property(c => c.Title).IsRequired().HasMaxLength(200); 
           e.Property(c => c.Description).IsRequired().HasMaxLength(2000); 
        });

        modelBuilder.Entity<Board>().Property(b => b.Name).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Column>().Property(c => c.Name).IsRequired().HasMaxLength(100);
    }
}