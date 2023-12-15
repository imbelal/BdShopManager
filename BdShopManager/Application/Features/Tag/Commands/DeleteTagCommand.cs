using Common.RequestWrapper;

namespace Application.Features.Tag.Commands
{
    public class DeleteTagCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
    }
}
