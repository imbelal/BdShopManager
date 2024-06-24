using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Tag.Commands
{
    public class UpdateTagCommadHandler : ICommandHandler<UpdateTagCommad, Guid>
    {
        private readonly ITagRepository _tagRepository;
        public UpdateTagCommadHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<IResponse<Guid>> Handle(UpdateTagCommad request, CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Category not found!!");

            tag.Title = request.Title;

            _tagRepository.Update(tag);
            await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(tag.Id);
        }
    }
}
