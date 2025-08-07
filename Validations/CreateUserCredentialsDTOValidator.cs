using FluentValidation;
using MinimalAPIPeliculas.DTOs;

namespace MinimalAPIPeliculas.Validations;

public class CreateUserCredentialsDTOValidator : AbstractValidator<UserCredentialsDTO>
{
    public CreateUserCredentialsDTOValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty();

    }
}