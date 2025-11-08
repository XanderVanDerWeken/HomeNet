namespace HomeNet.Core.Tests.Events;

using System.Threading.Tasks;
using HomeNet.Core.Events;

public class TestEventHandler : IEventHandler<TestEvent>
{
    public int HandleCount { get; private set; } = 0;

    public Task HandleAsync(TestEvent @event)
    {
        HandleCount++;
        
        return Task.CompletedTask;
    }
}
