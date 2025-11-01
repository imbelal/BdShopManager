using Common.UnitOfWork;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SalesReturnRepository : GenericRepository<SalesReturn>, ISalesReturnRepository
    {
        private readonly IApplicationDbContext _context;
        public SalesReturnRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
            _context = applicationDbContext;
        }

        public async override Task<SalesReturn> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _context.SalesReturns.Include(salesReturn => salesReturn.SalesReturnItems)
                                        .Where(salesReturn => salesReturn.Id == Id).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
