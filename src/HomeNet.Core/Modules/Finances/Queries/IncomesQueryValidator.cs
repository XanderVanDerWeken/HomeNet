
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Queries;

public sealed class IncomesQueryValidator : BaseValidator<IncomesQuery>
{
    protected override void ValidateInternal(IncomesQuery entity)
    {
        if (entity.Year < 2000 || entity.Year > 3000)
        {
            Errors.Add("Year must be between 2000 and 3000.");
        }

        if (entity.Month < 1 || entity.Month > 12)
        {
            Errors.Add("Month must be between 1 and 12.");
        }
    }
}
