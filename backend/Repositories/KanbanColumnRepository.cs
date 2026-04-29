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

        public async Task<List<KanbanColumn>> GetByUserIdAsync(int userId)
        {
            return await _context.KanbanColumns
                .Where(column => column.UserId == userId)
                .ToListAsync();
        }

        public async Task<KanbanColumn?> GetByIdAsync(int id)
        {
            return await _context.KanbanColumns.FindAsync(id);
        }

        public async Task<List<KanbanColumn>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.KanbanColumns
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<KanbanColumn> CreateAsync(KanbanColumn kanbanColumn)
        {
            _context.KanbanColumns.Add(kanbanColumn);
            await _context.SaveChangesAsync();
            return kanbanColumn;
        }

        public async Task<KanbanColumn> UpdateAsync(KanbanColumn kanbanColumn)
        {
            _context.KanbanColumns.Update(kanbanColumn);
            await _context.SaveChangesAsync();
            return kanbanColumn;
        }

        public async Task UpdateRangeAsync(IEnumerable<KanbanColumn> kanbanColumns)
        {
            _context.KanbanColumns.UpdateRange(kanbanColumns);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var kanbanColumn = await _context.KanbanColumns.FindAsync(id);

            if (kanbanColumn != null)
            {
                _context.KanbanColumns.Remove(kanbanColumn);
                await _context.SaveChangesAsync();
            }
        }
    }
}