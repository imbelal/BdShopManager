using Application.Features.Expense.DTOs;
using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Expense.Queries
{
    public class GetAllExpensesQueryHandler : IQueryHandler<GetAllExpensesQuery, Pagination<ExpenseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllExpensesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<ExpenseDto>>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Expenses.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x => x.Title.Contains(request.SearchTerm) ||
                                        x.Description.Contains(request.SearchTerm) ||
                                        x.Remarks.Contains(request.SearchTerm) ||
                                        (x.ReceiptNumber != null && x.ReceiptNumber.Contains(request.SearchTerm)));
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(x => x.ExpenseDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(x => x.ExpenseDate <= request.EndDate.Value);
            }

            if (request.ExpenseType.HasValue)
            {
                query = query.Where(x => (int)x.ExpenseType == request.ExpenseType.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => (int)x.Status == request.Status.Value);
            }

            if (!string.IsNullOrEmpty(request.PaymentMethod))
            {
                if (int.TryParse(request.PaymentMethod, out var paymentMethod))
                {
                    query = query.Where(x => (int)x.PaymentMethod == paymentMethod);
                }
            }

            // Apply ordering
            query = query.OrderByDescending(x => x.CreatedUtcDate);

            var expensesQuery = query
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
                    CreatedByUserName = x.CreatedBy.ToString() // You might want to include CreatedBy user details
                });

            var pagination = new Pagination<ExpenseDto>(expensesQuery, request.PageNumber, request.PageSize);
            return Response.Success(pagination);
        }
    }
}