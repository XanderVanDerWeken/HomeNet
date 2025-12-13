using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed record RecalculateMonthlyTimelineCommand : ICommand, IValidatable<RecalculateMonthlyTimelineCommand>
{
    public int Year { get; init; }

    public int Month { get; init; }

    public ValidationResult Validate()
        => new RecalculateMonthlyTimelineCommandValidator().Validate(this);
}
