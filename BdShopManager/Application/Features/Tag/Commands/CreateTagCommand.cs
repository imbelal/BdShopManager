using Common.RequestWrapper;

namespace Application.Features.Tag.Commands
{
    public class CreateTagCommand : ICommand<Guid>
    {
        public string Title { get; set; }
    }
}
