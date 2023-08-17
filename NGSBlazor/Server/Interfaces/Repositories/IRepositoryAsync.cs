using Infrastructure.Interfaces.Entities;

namespace NGSBlazor.Server.Interfaces.Repositories
{
    public interface IRepositoryAsync<T,TKey> where T : class
    {
        IQueryable<T> Entities { get; }

        Task<T?> GetByIdAsync(TKey id);

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);
    }
}
