using Domain.Enums;

namespace Domain.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int StockQuantity { get; set; }
        public ProductUnit Unit { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal ProfitMargin { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public List<string> ProductTags { get; set; }
        public List<ProductPhotoDto> ProductPhotos { get; set; } = new List<ProductPhotoDto>();
    }
}
