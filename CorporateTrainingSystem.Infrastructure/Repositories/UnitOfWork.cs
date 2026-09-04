using System.Collections.Concurrent;
using CorporateTrainingSystem.Domain.Interfaces;
using CorporateTrainingSystem.Infrastructure.Data;

namespace CorporateTrainingSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            return (IRepository<T>)_repositories.GetOrAdd(
                typeof(T),
                _ => new Repository<T>(_context));
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}