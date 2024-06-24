using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Tag.Commands
{
    public class DeleteTagCommandHandler : ICommandHandler<DeleteTagCommand, Guid>
    {
        private readonly ITagRepository _tagRepository;
        public DeleteTagCommandHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<IResponse<Guid>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.GetByIdAsync(request.Id);
            if (tag == null) throw new KeyNotFoundException();

            _tagRepository.Remove(tag);
            await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(tag.Id);
        }
    }
}
