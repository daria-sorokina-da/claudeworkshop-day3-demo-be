namespace StableApi.Validators;

using FluentValidation;
using StableApi.Models;

public class RetireHorseRequestValidator : AbstractValidator<RetireHorseRequest>
{
    public RetireHorseRequestValidator()
    {
        RuleFor(r => r.Reason).Length(5, 200);
    }
}
