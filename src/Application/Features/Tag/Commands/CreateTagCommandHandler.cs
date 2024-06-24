using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Tag.Commands
{
    public class CreateTagCommandHandler : ICommandHandler<CreateTagCommand, Guid>
    {
        private readonly ITagRepository _tagRepository;

        public CreateTagCommandHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IResponse<Guid>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var tag = new Domain.Entities.Tag();
            tag.Id = Guid.NewGuid();
            tag.Title = request.Title;

            _tagRepository.Add(tag);
            await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(tag.Id);
        }

    }
}
