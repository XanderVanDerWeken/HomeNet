namespace HomeNet.Modules.Chores.Services;

using HomeNet.Core.Common;
using HomeNet.Modules.Chores.Models;

public class ChoresService : IChoresService
{
    public Task<Result> CreateChoreSeries(ChoreSeries newChoreSeries)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteChoreSeries(int choreSeriesId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> CreateChore(Chore newChore)
    {
        throw new NotImplementedException();
    }

    public Task<Result> CompleteChore(int choreId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteChore(int choreId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Chore>>> GetChoresByUser(string username)
    {
        throw new NotImplementedException();
    }
}
