using FluentValidation;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Repositories;

namespace MinimalAPIPeliculas.Validations;

public class CreateGenreDTOValidator : AbstractValidator<CreateGenreDTO>
{
    public CreateGenreDTOValidator(
        IRepositoryGenres repository,
        IHttpContextAccessor httpContextAccessor
    )
    {
        var id = 0;
        var valuePathId = httpContextAccessor.HttpContext.Request.RouteValues["id"];

        if (valuePathId is string valueString)
        {
            int.TryParse(valueString, out id);
        }

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Must(FirstCapitalLetter)
            .MustAsync(async (Name, _) =>
            {
                var exists = await repository.Exists(id: id, Name);
                return !exists;
            });
    }

    private bool FirstCapitalLetter(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return char.IsUpper(value[0]);
    }
}