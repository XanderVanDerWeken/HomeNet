namespace HomeNet.Core.Common.Cqrs;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand
{
    Task<Result<TResult>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}
