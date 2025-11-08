using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Expense.Commands
{
    public class CreateExpenseCommandHandler : ICommandHandler<CreateExpenseCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateExpenseCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Guid>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = new Domain.Entities.Expense(
                request.Title,
                request.Description,
                request.Amount,
                request.Remarks,
                request.ExpenseDate,
                request.ExpenseType,
                request.PaymentMethod,
                request.ReceiptNumber
            );

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync(cancellationToken);

            return Response.Success<Guid>(expense.Id);
        }
    }
}