using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Enums;

namespace Domain.Entities
{
    public class Product : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        private List<ProductTag> productTags = new();
        private List<ProductPhoto> productPhotos = new();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public ProductUnit Unit { get; set; }
        public Guid CategoryId { get; set; }
        public ProductStatus Status { get; set; }
        public int StockQuantity { get; private set; } = 0;
        public decimal CostPrice { get; private set; } = 0; // Average buying price
        public decimal SellingPrice { get; set; } = 0; // Retail/selling price
        public bool IsDeleted { get; set; } = false;
        public Tenant Tenant { get; set; }

        // Calculated property for profit margin percentage
        public decimal ProfitMargin => SellingPrice > 0
            ? Math.Round(((SellingPrice - CostPrice) / SellingPrice) * 100, 2)
            : 0;

        public Product() : base()
        {

        }

        public Product(string title, string description, string size, string color, Guid categoryId, ProductUnit unit, decimal sellingPrice, List<Guid> tagIds) : base(Guid.NewGuid())
        {
            Title = title;
            Description = description;
            Size = size;
            Color = color;
            CategoryId = categoryId;
            Unit = unit;
            SellingPrice = sellingPrice;
            Status = ProductStatus.Active;
            AddTags(tagIds);
        }

        public void Update(string title, string description, string size, string color, Guid categoryId, ProductUnit unit, decimal sellingPrice)
        {
            Title = title;
            Description = description;
            Size = size;
            Color = color;
            CategoryId = categoryId;
            Unit = unit;
            SellingPrice = sellingPrice;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

        public IReadOnlyCollection<ProductTag> ProductTags
        {
            get => productTags;
        }

        public IReadOnlyCollection<ProductPhoto> ProductPhotos
        {
            get => productPhotos;
        }

        private void AddTags(List<Guid> tagIds)
        {
            productTags.AddRange(tagIds.Select(tagId => new ProductTag(this.Id, tagId)).ToList());
        }

        private void RemoveTags(List<Guid> tagIds)
        {
            productTags = productTags.Where(pt => !tagIds.Contains(pt.TagId)).ToList();
        }

        public void UpdateTags(List<Guid> tagIds)
        {
            List<Guid> existingTagIds = productTags.Select(pt => pt.TagId).ToList();
            List<Guid> tagsToBeAdded = tagIds.Except(existingTagIds).ToList();
            List<Guid> tagsToBeDeleted = existingTagIds.Except(tagIds).ToList();
            AddTags(tagsToBeAdded);
            RemoveTags(tagsToBeDeleted);
        }

        public void AddPhoto(ProductPhoto photo)
        {
            productPhotos.Add(photo);
        }

        public void RemovePhoto(Guid photoId)
        {
            var photo = productPhotos.FirstOrDefault(p => p.Id == photoId);
            if (photo != null)
            {
                productPhotos.Remove(photo);
            }
        }

        public void SetPrimaryPhoto(Guid photoId)
        {
            // Remove primary flag from all photos
            foreach (var photo in productPhotos)
            {
                if (photo.IsPrimary)
                {
                    photo.SetAsPrimary(false);
                }
            }

            // Set the specified photo as primary
            var targetPhoto = productPhotos.FirstOrDefault(p => p.Id == photoId);
            if (targetPhoto != null)
            {
                targetPhoto.SetAsPrimary(true);
            }
        }

        public void IncreaseStockQuantity(int quantity)
        {
            if (quantity > 0)
            {
                StockQuantity += quantity;
            }
        }

        public void DecreaseStockQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new Common.Exceptions.BusinessLogicException("Quantity to decrease must be greater than zero.");
            }

            if (StockQuantity < quantity)
            {
                throw new Common.Exceptions.BusinessLogicException($"Insufficient stock for product '{Title}'. Available: {StockQuantity}, Requested: {quantity}");
            }

            StockQuantity -= quantity;
        }

        public void UpdateAverageCost(decimal newCost, int newQuantity)
        {
            if (newCost < 0)
            {
                throw new Common.Exceptions.BusinessLogicException("Cost cannot be negative.");
            }

            if (newQuantity <= 0)
            {
                throw new Common.Exceptions.BusinessLogicException("Quantity must be greater than zero.");
            }

            if (StockQuantity == 0)
            {
                // First purchase - set cost directly
                CostPrice = newCost;
            }
            else
            {
                // Calculate weighted average cost
                decimal totalValue = (CostPrice * StockQuantity) + (newCost * newQuantity);
                decimal totalQuantity = StockQuantity + newQuantity;
                CostPrice = Math.Round(totalValue / totalQuantity, 2);
            }
        }

        public void SetSellingPriceByMargin(decimal targetMarginPercentage)
        {
            if (targetMarginPercentage < 0 || targetMarginPercentage >= 100)
            {
                throw new Common.Exceptions.BusinessLogicException("Profit margin must be between 0 and 100.");
            }

            if (CostPrice <= 0)
            {
                throw new Common.Exceptions.BusinessLogicException("Cannot calculate selling price without cost price.");
            }

            // SellingPrice = CostPrice / (1 - targetMargin/100)
            SellingPrice = Math.Round(CostPrice / (1 - (targetMarginPercentage / 100)), 2);
        }
    }
}
