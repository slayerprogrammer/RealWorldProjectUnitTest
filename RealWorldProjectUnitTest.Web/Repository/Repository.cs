using Microsoft.EntityFrameworkCore;
using RealWorldProjectUnitTest.Web.Models;

namespace RealWorldProjectUnitTest.Web.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly RealWorldProjectContext _context;
        private readonly DbSet<TEntity> _entities;

        public Repository(RealWorldProjectContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _entities.Update(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
