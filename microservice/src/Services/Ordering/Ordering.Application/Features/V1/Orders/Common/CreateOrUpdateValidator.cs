using FluentValidation;

namespace Ordering.Application.Features.V1.Orders.Common;

public class CreateOrUpdateValidator : AbstractValidator<CreateOrUpdateCommand>
{
    public CreateOrUpdateValidator()
    {
        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage("{FirstName} is required.")
            .MaximumLength(50).WithMessage("{FirstName} must not exceed 50 characters.");

        RuleFor(p => p.LastName)
            .NotEmpty()
            .WithMessage("{LastName} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{LastName} must not exceed 150 characters.");

        RuleFor(p => p.EmailAddress)
            .NotEmpty().WithMessage("{EmailAddress} is required.")
            .EmailAddress().WithMessage("{EmailAddress} is not a valid email address.");

        RuleFor(p => p.TotalPrice)
            .NotEmpty().WithMessage("{TotalPrice} is required.")
            .GreaterThan(0).WithMessage("{TotalPrice} must be greater than 0.");
    }
}
