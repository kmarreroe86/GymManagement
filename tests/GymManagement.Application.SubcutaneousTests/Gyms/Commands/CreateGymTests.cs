﻿using FluentAssertions;
using GymManagement.Api.Services;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Common.Models;
using GymManagement.Application.SubcutaneousTests.Common;
using GymManagement.Domain.Subscriptions;
using MediatR;
using NSubstitute;
using TestCommon.Gyms;
using TestCommon.Subscriptions;

namespace GymManagement.Application.SubcutaneousTests.Gyms.Commands
{
    [Collection(MediatorFactoryCollection.CollectionName)]
    public class CreateGymTests
    {
        private readonly MediatorFactory _mediatorFactory;
        private readonly IMediator _mediator;/* = mediatorFactory.CreateMediator();*/


        public CreateGymTests(MediatorFactory mediatorFactory)
        {
            _mediatorFactory = mediatorFactory;
            _mediator = _mediatorFactory.CreateMediator();
        }


        [Fact]
        public async Task CreateGym_WhenValidCommand_ShouldCreateGym()
        {
            // Arrange
            var subscription = await CreateSubscription();

            // Create a valid CreateGymCommand
            var createGymCommand = GymCommandFactory.CreateCreateGymCommand(subsriptionId: subscription.Id);

            // Act
            var createGymResult = await _mediator.Send(createGymCommand);

            // Assert
            createGymResult.IsError.Should().BeFalse();
            createGymResult.Value.SubscriptionId.Should().Be(subscription.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(200)]
        public async Task CreateGym_WhenCommandContainsInvalidData_ShouldReturnValidationError(int gymNameLength)
        {
            // Arrange
            string gymName = new('a', gymNameLength);
            var createGymCommand = GymCommandFactory.CreateCreateGymCommand(name: gymName);

            // Act
            var result = await _mediator.Send(createGymCommand);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Name");
        }



        private async Task<Subscription> CreateSubscription()
        {
            //  1. Create a CreateSubscriptionCommand
            var createSubscriptionCommand = SubscriptionCommandFactory.CreateCreateSubscriptionCommand();

            //  2. Sending it to MediatR
            var result = await _mediator.Send(createSubscriptionCommand);

            //  3. Making sure it was created successfully
            result.IsError.Should().BeFalse();
            return result.Value;
        }
    }
}
