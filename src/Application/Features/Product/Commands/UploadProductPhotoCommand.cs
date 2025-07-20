using Common.RequestWrapper;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Product.Commands
{
    public class UploadProductPhotoCommand : ICommand<Guid>
    {
        public Guid ProductId { get; set; }
        public IFormFile File { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }

        public UploadProductPhotoCommand(Guid productId, IFormFile file, bool isPrimary = false, int displayOrder = 0)
        {
            ProductId = productId;
            File = file;
            IsPrimary = isPrimary;
            DisplayOrder = displayOrder;
        }
    }
} 