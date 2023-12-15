using Common.Entities.Interfaces;
using Common.Repositories.Interfaces;
using Common.UnitOfWork;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class, IAggregateRoot
    {
        protected readonly IApplicationDbContext _applicationDbContext;
        protected readonly IUnitOfWork _unitOfWork;

        public IUnitOfWork UnitOfWork { get { return this._unitOfWork; } }

        public GenericRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork)
        {
            _applicationDbContext = applicationDbContext;
            _unitOfWork = unitOfWork;
        }

        // Define a method to get the DbSet for the entity
        protected DbSet<T> GetDbSet()
        {
            return _applicationDbContext.GetDbSet<T>();
        }

        public void Add(T entity)
        {
            GetDbSet().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            GetDbSet().AddRange(entities);
        }

        public void Update(T entity)
        {
            GetDbSet().Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            GetDbSet().UpdateRange(entities);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await GetDbSet().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid Id)
        {
            return await GetDbSet().FindAsync(Id);
        }

        public void Remove(T entity)
        {
            GetDbSet().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            GetDbSet().RemoveRange(entities);
        }
    }
}
