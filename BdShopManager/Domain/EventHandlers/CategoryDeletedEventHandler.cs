using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class CategoryDeletedEventHandler : INotificationHandler<CategoryDeletedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;
        public CategoryDeletedEventHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }
        public async Task Handle(CategoryDeletedEvent @event, CancellationToken cancellationToken)
        {
            List<Product> products = await _context.Products.Where(p => p.CategoryId == @event.CategoryId).ToListAsync(cancellationToken);
            _productRepository.RemoveRange(products);
        }
    }
}
