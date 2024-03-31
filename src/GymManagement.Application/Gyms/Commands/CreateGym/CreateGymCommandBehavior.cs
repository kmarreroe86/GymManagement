//using ErrorOr;
//using GymManagement.Domain.Gyms;
//using MediatR;

//namespace GymManagement.Application.Gyms.Commands.CreateGym
//{
//    /*Executes before CreateGymCommandHandler*/
//    public class CreateGymCommandBehavior : IPipelineBehavior<CreateGymCommand, ErrorOr<Gym>>
//    {
//        public async Task<ErrorOr<Gym>> Handle(
//            CreateGymCommand request,
//            RequestHandlerDelegate<ErrorOr<Gym>> next,
//            CancellationToken cancellationToken)
//        {
//            var validator = new CreateGymCommandValidator();
//            var validationResult = await validator.ValidateAsync(request, cancellationToken);
//            if (!validationResult.IsValid)
//            {
//                return validationResult.Errors.Select(err => Error.Validation(code: err.PropertyName, description: err.ErrorMessage)
//                ).ToList();
//            }

//            return await next();
//        }
//    }
//}
