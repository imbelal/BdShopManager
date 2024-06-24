using Common.ResponseWrapper;
using MediatR;

namespace Common.RequestWrapper
{
    public interface ICommand<T> : IRequest<IResponse<T>> { }

    public interface ICommandHandler<in TRequest, TResponse> :
        IRequestHandler<TRequest, IResponse<TResponse>> where TRequest : ICommand<TResponse>
    { }
}
