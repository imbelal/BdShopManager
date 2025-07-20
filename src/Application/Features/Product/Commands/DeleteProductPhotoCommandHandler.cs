using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Product.Commands
{
    public class DeleteProductPhotoCommandHandler : ICommandHandler<DeleteProductPhotoCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileStorageService _fileStorageService;

        public DeleteProductPhotoCommandHandler(
            IProductRepository productRepository,
            IFileStorageService fileStorageService)
        {
            _productRepository = productRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<IResponse<Guid>> Handle(DeleteProductPhotoCommand command, CancellationToken cancellationToken)
        {
            // Find the product that contains this photo
            var product = await _productRepository.GetByPhotoIdAsync(command.PhotoId, cancellationToken);

            if (product == null)
                return Response.Fail<Guid>("Product photo not found");

            var productPhoto = product.ProductPhotos.FirstOrDefault(p => p.Id == command.PhotoId);
            if (productPhoto == null)
                return Response.Fail<Guid>("Product photo not found");

            // Delete from Azure Storage
            await _fileStorageService.DeleteFileAsync(productPhoto.FileName, cancellationToken: cancellationToken);

            // Remove the photo from the product aggregate
            product.RemovePhoto(command.PhotoId);

            // Save changes - Entity Framework will now properly track the ProductPhoto as Deleted
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(productPhoto.Id);
        }
    }
}