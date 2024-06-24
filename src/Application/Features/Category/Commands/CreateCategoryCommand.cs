using Common.RequestWrapper;

namespace Application.Features.Category.Commands
{
    public class CreateCategoryCommand : ICommand<Guid>
    {
        public string Title { get; set; }

        public CreateCategoryCommand(string title)
        {
            Title = title;
        }
    }
}
