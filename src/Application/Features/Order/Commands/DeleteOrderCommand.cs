using Common.RequestWrapper;

namespace Application.Features.Order.Commands
{
    public class DeleteOrderCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }

        public DeleteOrderCommand(Guid id)
        {
            Id = id;
        }
    }
}
