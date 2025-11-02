using Common.RequestWrapper;
using Domain.Enums;

namespace Application.Features.Product.Commands
{
    public class UpdateProductCommand : ICommand<Guid>
    {
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public Guid CategoryId { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal SellingPrice { get; set; }


        public UpdateProductCommand(Guid productId, string title, string description, string size, string color, Guid categoryId, ProductUnit unit, decimal sellingPrice)
        {
            ProductId = productId;
            Title = title;
            Description = description;
            Size = size;
            Color = color;
            CategoryId = categoryId;
            Unit = unit;
            SellingPrice = sellingPrice;
        }
    }
}
