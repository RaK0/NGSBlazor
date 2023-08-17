using Domain.NGSContexts;
using Infrastructure.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using NGSBlazor.Server.Interfaces.Repositories;

namespace NGSBlazor.Server.Repositories
{
    public class RepositoryAsync<T, TKey> : IRepositoryAsync<T, TKey> where T : AEntity<TKey>
    {
        readonly NGSContext _context;
        public IQueryable<T> Entities => _context.Set<T>();

        public RepositoryAsync(NGSContext nGSContext)
        {
            _context = nGSContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context
                .Set<T>()
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(TKey id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            return await _context
               .Set<T>()
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .AsNoTracking()
               .ToListAsync();
        }

        public Task UpdateAsync(T entity)
        {
            T? exist = _context.Set<T>().Find(entity.Id);
            if (exist != null)
                _context.Entry(exist).CurrentValues.SetValues(entity);
            return Task.CompletedTask;
        }
    }
}
