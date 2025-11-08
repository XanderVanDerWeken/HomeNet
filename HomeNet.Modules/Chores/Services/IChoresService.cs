namespace HomeNet.Modules.Chores.Services;

using HomeNet.Core.Common;
using HomeNet.Modules.Chores.Models;

public interface IChoresService
{
    Task<Result> CreateChoreSeries(ChoreSeries newChoreSeries);

    Task<Result> DeleteChoreSeries(int choreSeriesId);

    Task<Result> CreateChore(Chore newChore);

    Task<Result> CompleteChore(int choreId);

    Task<Result> DeleteChore(int choreId);

    Task<Result<IEnumerable<Chore>>> GetChoresByUser(string username);
}
