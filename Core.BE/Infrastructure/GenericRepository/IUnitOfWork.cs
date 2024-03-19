using mConnect.Framework.Infrastructure.GenericRepository;

namespace Emeint.Common.Infrastructure.GenericRepository
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <param name="throwExc"></param>
        /// <returns>
        /// The number of objects written to the underlying database.
        /// </returns>
        int SaveChanges(bool throwExc = false);

        //Task<int> SaveChangesAsync();

        //Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        void Dispose();

        void BeginTransaction();

        void Rollback();

        int Commit();
        //Task<int> CommitAsync();

        IRepository<T> Repository<T>() where T : class;
    }
}
