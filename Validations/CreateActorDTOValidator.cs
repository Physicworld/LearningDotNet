using FluentValidation;
using MinimalAPIPeliculas.DTOs;

namespace MinimalAPIPeliculas.Validations;

public class CreateActorDTOValidator : AbstractValidator<CreateActorDTO>
{
    public CreateActorDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
        var minDate = new DateTime(1900, 1, 1);
        RuleFor(x => x.Birthday)
            .LessThanOrEqualTo(DateTime.Now)
            .GreaterThanOrEqualTo(minDate);
    }
}