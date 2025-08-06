using FluentValidation;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Endpoints;

namespace MinimalAPIPeliculas.Validations;

public class CreateMovieDTOValidator : AbstractValidator<CreateMovieDTO>
{
    public CreateMovieDTOValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(50);
    }
}