using System.Data;
using FluentValidation;
using MinimalAPIPeliculas.DTOs;

namespace MinimalAPIPeliculas.Validations;

public class CreateCommentDTOValidator : AbstractValidator<CreateCommentDTO>
{
    public CreateCommentDTOValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty();
    }
}