namespace QBReconcile.Services;

public interface IRequest<TResult>
{

}

public interface IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    public Task<TResult> Handle(TRequest request, CancellationToken cancellationToken = default);
}
