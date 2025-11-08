using Application.Features.Expense.DTOs;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Common.Services.Interfaces;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Features.Expense.Queries
{
    public class GetExpensesPdfQueryHandler : IQueryHandler<GetExpensesPdfQuery, byte[]>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        private readonly IPdfGeneratorService _pdfGeneratorService;
        private readonly ICurrentUserService _currentUserService;

        public GetExpensesPdfQueryHandler(IReadOnlyApplicationDbContext context, IPdfGeneratorService pdfGeneratorService, ICurrentUserService currentUserService)
        {
            _context = context;
            _pdfGeneratorService = pdfGeneratorService;
            _currentUserService = currentUserService;
        }

        public async Task<IResponse<byte[]>> Handle(GetExpensesPdfQuery query, CancellationToken cancellationToken)
        {
            // Get expenses within the date period
            var expensesQuery = _context.Expenses.AsQueryable();

            if (query.StartDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.ExpenseDate >= query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(x => x.ExpenseDate <= query.EndDate.Value);
            }

            var expenses = await expensesQuery
                .OrderByDescending(x => x.CreatedUtcDate)
                .Select(x => new ExpenseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Amount = x.Amount,
                    Remarks = x.Remarks,
                    ExpenseDate = x.ExpenseDate,
                    ExpenseType = (int)x.ExpenseType,
                    ExpenseTypeName = x.ExpenseType.ToString(),
                    Status = (int)x.Status,
                    StatusName = x.Status.ToString(),
                    PaymentMethod = (int)x.PaymentMethod,
                    PaymentMethodName = x.PaymentMethod.ToString(),
                    ReceiptNumber = x.ReceiptNumber,
                    PaidDate = x.PaidDate,
                    ApprovedByUserName = x.ApprovedBy,
                    ApprovedDate = x.ApprovedDate,
                    CreatedDate = x.CreatedUtcDate.DateTime,
                    CreatedByUserName = x.CreatedBy.ToString()
                })
                .ToListAsync(cancellationToken);

            if (!expenses.Any())
            {
                throw new Common.Exceptions.BusinessLogicException("No expenses found in the specified date range!");
            }

            // Get tenant details from the current user's tenant claim
            var tenantIdClaim = _currentUserService?.GetUser()?.Claims?.FirstOrDefault(c => c.Type == "tenantId")?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim))
            {
                throw new Common.Exceptions.BusinessLogicException("Tenant ID not found in user claims.");
            }
            Guid tenantId = Guid.Parse(tenantIdClaim);
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
            if (tenant == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Tenant not found!");
            }

            // Generate PDF
            var pdfBytes = _pdfGeneratorService.GenerateExpensesPdf(
                expenses.Cast<object>().ToList(),
                tenant.Name,
                tenant.Address,
                tenant.PhoneNumber,
                query.StartDate,
                query.EndDate
            );

            return Response.Success(pdfBytes);
        }
    }
}