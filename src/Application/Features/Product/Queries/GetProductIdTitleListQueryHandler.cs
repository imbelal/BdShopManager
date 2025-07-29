using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetProductIdTitleListQueryHandler : IRequestHandler<GetProductIdTitleListQuery, List<ProductIdTitleDto>>
{
    private readonly IReadOnlyApplicationDbContext _context;

    public GetProductIdTitleListQueryHandler(IReadOnlyApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductIdTitleDto>> Handle(GetProductIdTitleListQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Select(p => new ProductIdTitleDto { Id = p.Id, Title = p.Title })
            .ToListAsync(cancellationToken);
    }
}
