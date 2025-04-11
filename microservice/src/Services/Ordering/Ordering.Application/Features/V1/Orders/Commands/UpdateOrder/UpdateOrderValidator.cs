using FluentValidation;

namespace Ordering.Application.Features.V1.Orders;

public class UpdateOrderValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("{Id} is required.");
    }
}
