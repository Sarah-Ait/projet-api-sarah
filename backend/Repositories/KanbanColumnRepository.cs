using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Interfaces;
using backend.Models;

namespace backend.Repositories
{
    public class KanbanColumnRepository : IKanbanColumnRepository
    {
        private readonly AppDbContext _context;

        public KanbanColumnRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<KanbanColumn>> GetAllAsync()
        {
            return await _context.KanbanColumns.ToListAsync();
        }

        public async Task<KanbanColumn?> GetByIdAsync(int id)
        {
            return await _context.KanbanColumns.FindAsync(id);
        }

        public async Task<KanbanColumn> CreateAsync(KanbanColumn kanbanColumn)
        {
            _context.KanbanColumns.Add(kanbanColumn);
            await _context.SaveChangesAsync();
            return kanbanColumn;
        }
    }
}