using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;

namespace HomeNet.Core.Modules.Cards.Commands;

public class AddCardCommandHandler : ICommandHandler<AddCardCommand>
{
    public Task<Result> HandleAsync(
        AddCardCommand command, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
