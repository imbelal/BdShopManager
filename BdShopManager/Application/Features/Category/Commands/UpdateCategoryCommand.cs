using Common.RequestWrapper;

namespace Application.Features.Category.Commands
{
    public class UpdateCategoryCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public UpdateCategoryCommand(Guid id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
