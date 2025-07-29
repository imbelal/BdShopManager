using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Product.Commands
{
    public class SetPrimaryProductPhotoCommandHandler : ICommandHandler<SetPrimaryProductPhotoCommand, Guid>
    {
        private readonly IProductRepository _productRepository;

        public SetPrimaryProductPhotoCommandHandler(
            IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IResponse<Guid>> Handle(SetPrimaryProductPhotoCommand command, CancellationToken cancellationToken)
        {
            // Find the product that contains this photo
            var product = await _productRepository.GetByPhotoIdAsync(command.PhotoId, cancellationToken);

            if (product == null)
                return Response.Fail<Guid>("Product photo not found");

            var targetPhoto = product.ProductPhotos.FirstOrDefault(p => p.Id == command.PhotoId);
            if (targetPhoto == null)
                return Response.Fail<Guid>("Product photo not found");

            // Use the product aggregate method to set the primary photo
            product.SetPrimaryPhoto(command.PhotoId);

            // Save changes - Entity Framework will now properly track the ProductPhoto as Modified
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(targetPhoto.Id);
        }
    }
}