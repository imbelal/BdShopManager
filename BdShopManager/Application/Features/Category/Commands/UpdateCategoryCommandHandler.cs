using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Category.Commands
{
    public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IResponse<Guid>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Category category = await _categoryRepository.GetByIdAsync(command.Id);
            if (category == null) throw new Exception("Category not found!!");

            category.Title = command.Title;
            category.RaiseUpdatedEvent();
            _categoryRepository.Update(category);
            await _categoryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(category.Id);
        }
    }
}
