using Common.RequestWrapper;

namespace Application.Features.Product.Commands
{
    public class SetPrimaryProductPhotoCommand : ICommand<Guid>
    {
        public Guid PhotoId { get; set; }

        public SetPrimaryProductPhotoCommand(Guid photoId)
        {
            PhotoId = photoId;
        }
    }
} 