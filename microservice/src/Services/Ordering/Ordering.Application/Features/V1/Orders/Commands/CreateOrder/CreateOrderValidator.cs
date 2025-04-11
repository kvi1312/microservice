using FluentValidation;

namespace Ordering.Application.Features.V1.Orders;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserName).
            NotEmpty().WithMessage("UserName is required.").
            MaximumLength(50).WithMessage("UserName must not exceed 50 characters.");
        RuleFor(x => x.TotalPrice).
            NotEmpty().WithMessage("TotalPrice is required.").
            GreaterThan(0).WithMessage("TotalPrice must be greater than 0.");
        RuleFor(x => x.EmailAddress).
            NotEmpty().WithMessage("EmailAddress is required.").
            MaximumLength(50).WithMessage("EmailAddress must not exceed 50 characters.").
            EmailAddress().WithMessage("EmailAddress is not a valid email address.");
        RuleFor(x => x.ShippingAddress).
            NotEmpty().WithMessage("ShippingAddress is required.").
            MaximumLength(200).WithMessage("ShippingAddress must not exceed 200 characters.");
    }
}
