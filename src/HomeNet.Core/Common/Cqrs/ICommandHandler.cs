namespace HomeNet.Core.Common.Cqrs;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}
