using ErrorOr;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GymManagement.Application.Common.Behaviors;
using GymManagement.Application.Gyms.Commands.CreateGym;
using GymManagement.Domain.Gyms;
using MediatR;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using TestCommon.Gyms;

namespace GymManagement.Application.UnitTests.Common.Behaviors
{
    public class ValidationBehaviorTests
    {
        private readonly ValidationBehavior<CreateGymCommand, ErrorOr<Gym>> _validationBehavior;
        private readonly RequestHandlerDelegate<ErrorOr<Gym>> _mockNextBehavior;
        private readonly IValidator<CreateGymCommand> _mockValidator;


        public ValidationBehaviorTests()
        {
            _mockValidator = Substitute.For<IValidator<CreateGymCommand>>();
            _validationBehavior = new ValidationBehavior<CreateGymCommand, ErrorOr<Gym>>(_mockValidator);
            _mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<Gym>>>();
        }


        [Fact]
        async Task InvokeBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
        {
            // Arrange
            var createGymRequest = GymCommandFactory.CreateCreateGymCommand();
            var gym = GymsFactory.CreateGym();

            _mockValidator.ValidateAsync(createGymRequest, Arg.Any<CancellationToken>()) // Mock validation return no errors
                 .Returns(new ValidationResult());

            _mockNextBehavior.Invoke().Returns(gym);     // calling next behavior returns a gym


            // Act
            var result = await _validationBehavior.Handle(createGymRequest, _mockNextBehavior, default);


            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(gym);
        }

        [Fact]
        async Task InvokeBehavior_WhenValidatorResultIsNotValid_ShouldReturnListOfErrors()
        {
            // Arrange
            var createGymRequest = GymCommandFactory.CreateCreateGymCommand();
            List<ValidationFailure> validationFailures = new() { new ValidationFailure(propertyName: "foo", errorMessage: "bad foo") };

            _mockValidator.ValidateAsync(createGymRequest, Arg.Any<CancellationToken>()) // Mock validation with errors
                 .Returns(new ValidationResult(validationFailures));

            // Act
            var result = await _validationBehavior.Handle(createGymRequest, _mockNextBehavior, default);


            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("foo");
            result.FirstError.Description.Should().Be("bad foo");
        }
    }
}
