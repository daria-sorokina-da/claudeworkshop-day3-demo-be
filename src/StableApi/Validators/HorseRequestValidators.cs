using FluentValidation;
using StableApi.Models;

namespace StableApi.Validators;

public class CreateHorseRequestValidator : AbstractValidator<CreateHorseRequest>
{
    public CreateHorseRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.OwnerEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Breed).NotEmpty();
    }
}

public class UpdateHorseRequestValidator : AbstractValidator<UpdateHorseRequest>
{
    public UpdateHorseRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.OwnerEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Breed).NotEmpty();
    }
}

public class RetireHorseRequestValidator : AbstractValidator<RetireHorseRequest>
{
    public RetireHorseRequestValidator() =>
        RuleFor(x => x.Reason).NotEmpty().MinimumLength(5).MaximumLength(200);
}
