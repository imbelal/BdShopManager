using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class CategoryDeletedEventHandler : INotificationHandler<CategoryDeletedEvent>
    {
        private readonly IPostRepository _postRepository;
        private readonly IReadOnlyApplicationDbContext _context;
        public CategoryDeletedEventHandler(IPostRepository postRepository, IReadOnlyApplicationDbContext context)
        {
            _postRepository = postRepository;
            _context = context;
        }
        public async Task Handle(CategoryDeletedEvent @event, CancellationToken cancellationToken)
        {
            List<Product> posts = await _context.Products.Where(p => p.CategoryId == @event.CategoryId).ToListAsync(cancellationToken);
            _postRepository.RemoveRange(posts);
        }
    }
}
