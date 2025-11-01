using Common.UnitOfWork;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class StockAdjustmentRepository : GenericRepository<StockAdjustment>, IStockAdjustmentRepository
    {
        public StockAdjustmentRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
        }
    }
}
