using FluentValidation;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Repositories;

namespace MinimalAPIPeliculas.Validations;

public class CreateGenreDTOValidator : AbstractValidator<CreateGenreDTO>
{
    public CreateGenreDTOValidator(IRepositoryGenres repository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Must(FirstCapitalLetter)
            .MustAsync(async (Name, _) =>
            {
                var exists = await repository.Exists(id: 0, Name);
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