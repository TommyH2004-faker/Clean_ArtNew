using FluentValidation;

namespace TodoApp.Application.Features.GenreHandle.Command.Update
{
    public class UpdateGenreCommandValidator : AbstractValidator<UpdateGenreCommand>
    {
        public UpdateGenreCommandValidator()
        {
            RuleFor(x => x.IdGenre)
                .GreaterThan(0).WithMessage("IdGenre phải lớn hơn 0");

            RuleFor(x => x.NameGenre)
                .NotEmpty().WithMessage("Tên thể loại không được để trống")
                .MaximumLength(100).WithMessage("Tên thể loại không được vượt quá 100 ký tự");
        }
    }
}
