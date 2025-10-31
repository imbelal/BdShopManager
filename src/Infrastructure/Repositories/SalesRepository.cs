using Common.UnitOfWork;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SalesRepository : GenericRepository<Sales>, ISalesRepository
    {
        private readonly IApplicationDbContext _context;
        public SalesRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
            _context = applicationDbContext;
        }

        public async override Task<Sales> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _context.Sales.Include(sales => sales.SalesItems)
                                        .Where(sales => sales.Id == Id).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
