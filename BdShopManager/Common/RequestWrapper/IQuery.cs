using Common.ResponseWrapper;
using MediatR;

namespace Common.RequestWrapper
{
    public interface IQuery<T> : IRequest<IResponse<T>> { }

    public interface IQueryHandler<in TRequest, TResponse> :
        IRequestHandler<TRequest, IResponse<TResponse>> where TRequest : IQuery<TResponse>
    { }
}
