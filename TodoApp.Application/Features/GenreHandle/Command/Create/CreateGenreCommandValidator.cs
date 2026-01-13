using FluentValidation;

namespace TodoApp.Application.Features.GenreHandle.Command.Create
{
    public class CreateGenreCommandValidator : AbstractValidator<CreateGenreCommand>
    {
        public CreateGenreCommandValidator()
        {
            RuleFor(x => x.NameGenre)
                .NotEmpty().WithMessage("Tên thể loại không được để trống")
                .MaximumLength(100).WithMessage("Tên thể loại không được vượt quá 100 ký tự");
        }
    }
}
