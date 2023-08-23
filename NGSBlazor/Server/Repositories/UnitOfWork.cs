using Domain.NGSContexts;
using Infrastructure.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using NGSBlazor.Server.Interfaces.Repositories;
using System.Collections;

namespace NGSBlazor.Server.Repositories
{
    public class UnitOfWork<TKey> : IUnitOfWork<TKey>
    {
        readonly NGSContext _context;
        readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(NGSContext context)
        {
            _context = context;
        }

        public IRepositoryAsync<T, TKey> Repository<T>() where T : AEntity<TKey>
        {
            Type type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                Type repositoryType = typeof(RepositoryAsync<,>).MakeGenericType(type, typeof(TKey));
                object? repositoryInstance = Activator.CreateInstance(repositoryType, _context);
                if(repositoryInstance != null)
                    _repositories[type] = repositoryInstance;                
            }
            return (IRepositoryAsync<T, TKey>)_repositories[type];
        }
        public async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<int> CommitAndRemoveCacheAsync(CancellationToken cancellationToken, params string[] cacheKeys)
        {
            throw new NotImplementedException();
        }       
        public Task RollbackAsync()
        {
            _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            return Task.CompletedTask;
        }
    }
}
