using Microsoft.EntityFrameworkCore; // on importe EF core
using backend.Models;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<KanbanColumn> KanbanColumns { get; set; }

    public DbSet<Ticket> Tickets { get; set; }
} // pont entre bdd et c#  (relier les modèles à la base)