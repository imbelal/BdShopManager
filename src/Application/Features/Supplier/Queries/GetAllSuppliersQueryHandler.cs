using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Supplier.Queries
{
    public class GetAllSuppliersQueryHandler : IQueryHandler<GetAllSuppliersQuery, List<Domain.Entities.Supplier>>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        public GetAllSuppliersQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<Domain.Entities.Supplier>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.Supplier> suppliers = await _context.Suppliers.ToListAsync(cancellationToken);
            return Response.Success(suppliers);
        }
    }
}
