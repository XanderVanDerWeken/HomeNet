namespace HomeNet.Core.Common.Cqrs;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery
{
    Task<Result<TResult>> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken = default);
}
