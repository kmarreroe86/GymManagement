using FluentValidation;

namespace GymManagement.Application.Gyms.Commands.CreateGym
{
    public class CreateGymCommandValidator : AbstractValidator<CreateGymCommand>
    {
        public CreateGymCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);
                //.WithMessage("{PropertyName} is not valid. Cannot be empty less than 3 characters or more than 100 characters");
        }
    }
}
