using Application.Features.Expense.DTOs;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Expense.Queries
{
    public class GetExpenseByIdQueryHandler : IQueryHandler<GetExpenseByIdQuery, ExpenseDto>
    {
        private readonly IApplicationDbContext _context;

        public GetExpenseByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<ExpenseDto>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (expense == null)
            {
                return Response.Fail<ExpenseDto>("Expense not found.");
            }

            var expenseDto = new ExpenseDto
            {
                Id = expense.Id,
                Title = expense.Title,
                Description = expense.Description,
                Amount = expense.Amount,
                Remarks = expense.Remarks,
                ExpenseDate = expense.ExpenseDate,
                ExpenseType = (int)expense.ExpenseType,
                ExpenseTypeName = expense.ExpenseType.ToString(),
                Status = (int)expense.Status,
                StatusName = expense.Status.ToString(),
                PaymentMethod = (int)expense.PaymentMethod,
                PaymentMethodName = expense.PaymentMethod.ToString(),
                ReceiptNumber = expense.ReceiptNumber,
                PaidDate = expense.PaidDate,
                ApprovedByUserName = expense.ApprovedBy,
                ApprovedDate = expense.ApprovedDate,
                CreatedDate = expense.CreatedUtcDate.DateTime,
                CreatedByUserName = expense.CreatedBy.ToString() // You might want to include CreatedBy user details
            };

            return Response.Success(expenseDto);
        }
    }
}