using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Category.Commands
{
    public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IResponse<Guid>> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Category category = await _categoryRepository.GetByIdAsync(command.Id);
            if (category == null) throw new Exception("Category not found!!");

            _categoryRepository.Remove(category);
            category.RaiseDeleteEvent();
            await _categoryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(category.Id);
        }
    }
}
