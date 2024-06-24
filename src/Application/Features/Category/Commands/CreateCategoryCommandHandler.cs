using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Category.Commands
{
    public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IResponse<Guid>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Category category = new(command.Title);
            _categoryRepository.Add(category);
            category.RaiseCreatedEvent();
            await _categoryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(category.Id);
        }
    }
}
