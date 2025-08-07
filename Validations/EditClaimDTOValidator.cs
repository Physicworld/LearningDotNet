using FluentValidation;
using MinimalAPIPeliculas.DTOs;

namespace MinimalAPIPeliculas.Validations;

public class EditClaimDTOValidator : AbstractValidator<EditClaimDTO>
{
    public EditClaimDTOValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();
    }
}