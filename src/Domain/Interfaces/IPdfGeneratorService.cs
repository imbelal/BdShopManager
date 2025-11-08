using Domain.Dtos;

namespace Domain.Interfaces
{
    public interface IPdfGeneratorService
    {
        byte[] GenerateSalesPdf(SalesDto sales, string tenantName, string tenantAddress, string tenantPhone, string customerAddress, string customerPhone);
        byte[] GenerateExpensesPdf<T>(List<T> expenses, string tenantName, string tenantAddress, string tenantPhone, DateTime? startDate, DateTime? endDate) where T : class;
    }
}
