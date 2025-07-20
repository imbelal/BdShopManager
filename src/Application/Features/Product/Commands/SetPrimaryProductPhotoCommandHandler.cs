using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Commands
{
    public class SetPrimaryProductPhotoCommandHandler : ICommandHandler<SetPrimaryProductPhotoCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IApplicationDbContext _applicationDbContext;

        public SetPrimaryProductPhotoCommandHandler(
            IProductRepository productRepository,
            IApplicationDbContext applicationDbContext)
        {
            _productRepository = productRepository;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IResponse<Guid>> Handle(SetPrimaryProductPhotoCommand command, CancellationToken cancellationToken)
        {
            // Find the product that contains this photo
            var product = await _applicationDbContext.Products
                .Include(p => p.ProductPhotos)
                .FirstOrDefaultAsync(p => p.ProductPhotos.Any(pp => pp.Id == command.PhotoId), cancellationToken);
            
            if (product == null)
                return Response.Fail<Guid>("Product photo not found");

            var targetPhoto = product.ProductPhotos.FirstOrDefault(p => p.Id == command.PhotoId);
            if (targetPhoto == null)
                return Response.Fail<Guid>("Product photo not found");

            // Use the product aggregate method to set the primary photo
            product.SetPrimaryPhoto(command.PhotoId);

            // Explicitly update the ProductPhoto in the DbContext to ensure it's tracked as Modified
            _applicationDbContext.ProductPhotos.Update(targetPhoto);

            // Save changes - Entity Framework will now properly track the ProductPhoto as Modified
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(targetPhoto.Id);
        }
    }
} 