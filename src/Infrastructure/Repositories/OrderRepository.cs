using Common.UnitOfWork;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly IApplicationDbContext _context;
        public OrderRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
            _context = applicationDbContext;
        }

        public async override Task<Order> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders.Include(order => order.OrderDetails)
                                        .Where(order => order.Id == Id).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
