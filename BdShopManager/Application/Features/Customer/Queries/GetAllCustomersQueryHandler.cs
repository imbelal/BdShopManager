using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Customer.Queries
{
    public class GetAllCustomersQueryHandler : IQueryHandler<GetAllCustomersQuery, List<Domain.Entities.Customer>>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        public GetAllCustomersQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<Domain.Entities.Customer>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.Customer> suppliers = await _context.Customers.ToListAsync(cancellationToken);
            return Response.Success(suppliers);
        }
    }
}
