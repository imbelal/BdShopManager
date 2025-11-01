using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Category.Commands
{
    public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IReadOnlyApplicationDbContext context)
        {
            _categoryRepository = categoryRepository;
            _context = context;
        }
        public async Task<IResponse<Guid>> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Category category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (category == null) throw new Common.Exceptions.BusinessLogicException("Category not found!!");

            // Check if there are any products associated with this category
            var productCount = await _context.Products
                .CountAsync(p => p.CategoryId == command.Id && !p.IsDeleted, cancellationToken);

            if (productCount > 0)
            {
                throw new Common.Exceptions.BusinessLogicException(
                    $"Cannot delete category '{category.Title}' because it has {productCount} associated product(s). " +
                    "Please delete or reassign all products before deleting this category.");
            }

            _categoryRepository.Remove(category);
            await _categoryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(category.Id);
        }
    }
}
