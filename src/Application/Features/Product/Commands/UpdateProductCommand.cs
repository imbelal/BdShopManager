using Common.RequestWrapper;
using Domain.Enums;

namespace Application.Features.Product.Commands
{
    public class UpdateProductCommand : ICommand<Guid>
    {
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal SellingPrice { get; set; }


        public UpdateProductCommand(Guid productId, string title, string description, Guid categoryId, ProductUnit unit, decimal sellingPrice)
        {
            ProductId = productId;
            Title = title;
            Description = description;
            CategoryId = categoryId;
            Unit = unit;
            SellingPrice = sellingPrice;
        }
    }
}
