using Common.RequestWrapper;

namespace Application.Features.Product.Commands
{
    public class DeleteProductPhotoCommand : ICommand<Guid>
    {
        public Guid PhotoId { get; set; }

        public DeleteProductPhotoCommand(Guid photoId)
        {
            PhotoId = photoId;
        }
    }
} 