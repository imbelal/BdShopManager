using Domain.Enums;

namespace Domain.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public ProductUnit Unit { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public List<string> ProductTags { get; set; }
    }
}
