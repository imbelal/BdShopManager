using Common.Exceptions;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Expense.Commands
{
    public class DeleteExpenseCommandHandler : ICommandHandler<DeleteExpenseCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public DeleteExpenseCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Guid>> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (expense == null)
            {
                throw new BusinessLogicException("Expense not found.");
            }

            if (!expense.CanBeEdited())
            {
                throw new BusinessLogicException($"Expense cannot be deleted in {expense.Status} status.");
            }

            expense.Delete();
            await _context.SaveChangesAsync(cancellationToken);

            return Response.Success<Guid>(expense.Id);
        }
    }
}