using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Expense.Commands
{
    public class UpdateExpenseCommandHandler : ICommandHandler<UpdateExpenseCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public UpdateExpenseCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Guid>> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (expense == null)
            {
                return Response.Fail<Guid>("Expense not found.");
            }

            if (!expense.CanBeEdited())
            {
                return Response.Fail<Guid>($"Expense cannot be edited in {expense.Status} status.");
            }

            expense.Update(
                request.Title,
                request.Description,
                request.Amount,
                request.Remarks,
                request.ExpenseDate,
                request.ExpenseType,
                request.PaymentMethod,
                request.ReceiptNumber
            );

            await _context.SaveChangesAsync(cancellationToken);

            return Response.Success<Guid>(expense.Id);
        }
    }
}