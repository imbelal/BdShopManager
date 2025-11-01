using Domain.Enums;

namespace Domain.Dtos
{
    public class StockTransactionDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public StockTransactionType Type { get; set; }
        public string TypeName { get; set; }
        public StockReferenceType RefType { get; set; }
        public string RefTypeName { get; set; }
        public Guid RefId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string TransactionDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
