using Common.RequestWrapper;

namespace Application.Features.Tag.Commands
{
    public class UpdateTagCommad : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
