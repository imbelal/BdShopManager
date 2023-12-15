using Common.RequestWrapper;

namespace Application.Features.Category.Commands
{
    public class DeleteCategoryCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }

        public DeleteCategoryCommand(Guid id)
        {
            Id = id;
        }
    }
}
