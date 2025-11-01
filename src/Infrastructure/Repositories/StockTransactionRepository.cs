using Common.UnitOfWork;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class StockTransactionRepository : GenericRepository<Domain.Entities.StockTransaction>, IStockTransactionRepository
    {
        public StockTransactionRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
        }
    }
}
