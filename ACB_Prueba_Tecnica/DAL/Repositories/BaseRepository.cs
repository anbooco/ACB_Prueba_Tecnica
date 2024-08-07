using ACB_Prueba_Tecnica.DAL.Context;
using ACB_Prueba_Tecnica.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ACB_Prueba_Tecnica.DAL.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ACBContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ACBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByCondition(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<List<T>> GetAllByCondition(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task Add(T entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task UpdatePartial(T entity, params Expression<Func<T, object>>[] updatedProperties)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;

            // Solo marcar las propiedades especificadas como modificadas
            foreach (var property in updatedProperties) {
                entry.Property(property).IsModified = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChangesAsync();
        }

        public async Task Deactivate(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
