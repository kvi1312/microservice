using FluentValidation;

namespace Ordering.Application.Features.V1.Orders;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserName).
            NotEmpty().WithMessage("UserName is required.").
            MaximumLength(50).WithMessage("UserName must not exceed 50 characters.");
    }
}
