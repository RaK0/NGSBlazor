using Infrastructure.Interfaces.Entities;
using NGSBlazor.Server.Repositories;
using System.Security.Cryptography;

namespace NGSBlazor.Server.Interfaces.Repositories
{
    public interface IUnitOfWork<TKey>
    {
        IRepositoryAsync<T, TKey> Repository<T>() where T : AEntity<TKey>;
        Task<int> Commit(CancellationToken cancellationToken);
        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);
        Task Rollback();
    }
}
