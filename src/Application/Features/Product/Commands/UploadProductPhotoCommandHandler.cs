using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Product.Commands
{
    public class UploadProductPhotoCommandHandler : ICommandHandler<UploadProductPhotoCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileStorageService _fileStorageService;

        public UploadProductPhotoCommandHandler(
            IProductRepository productRepository,
            IFileStorageService fileStorageService)
        {
            _productRepository = productRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<IResponse<Guid>> Handle(UploadProductPhotoCommand command, CancellationToken cancellationToken)
        {
            // Verify product exists and load it with photos through the repository
            var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);
            if (product == null)
                return Response.Fail<Guid>("Product not found");

            // Validate file
            if (command.File == null || command.File.Length == 0)
                return Response.Fail<Guid>("No file provided");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(command.File.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
                return Response.Fail<Guid>("Invalid file type. Only image files are allowed.");

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";

            // Upload to Azure Storage
            string blobUrl = string.Empty;
            using (var stream = command.File.OpenReadStream())
            {
                blobUrl = await _fileStorageService.UploadFileAsync(
                    stream,
                    fileName,
                    command.File.ContentType,
                    cancellationToken: cancellationToken);
            }

            // Create ProductPhoto entity
            var productPhoto = new Domain.Entities.ProductPhoto(
                command.ProductId,
                fileName,
                command.File.FileName,
                command.File.ContentType,
                command.File.Length,
                blobUrl,
                command.IsPrimary,
                command.DisplayOrder
            );

            // Add the new photo to the product
            product.AddPhoto(productPhoto);

            // If this photo should be primary, set it as primary (this will handle removing primary from others)
            if (command.IsPrimary)
            {
                product.SetPrimaryPhoto(productPhoto.Id);
            }

            // Save changes - Entity Framework will now properly track the ProductPhoto as Added
            _productRepository.Update(product);
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(productPhoto.Id);
        }
    }
}