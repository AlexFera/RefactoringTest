using AutoFixture;
using FluentAssertions;
using LegacyApp.CreditLimitProviders;
using LegacyApp.DataAccess;
using LegacyApp.Models;
using LegacyApp.Repositories;
using LegacyApp.Services;
using LegacyApp.Validators;
using NSubstitute;
using System;
using Xunit;

namespace LegacyApp.UnitTests
{
    public class UserServiceTests
    {
        private readonly UserService _sut;
        private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();
        private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
        private readonly IUserCreditService _userCreditService = Substitute.For<IUserCreditService>();
        private readonly IUserDataAccess _userDataAccess = Substitute.For<IUserDataAccess>();
        private readonly IFixture _fixture = new Fixture();

        public UserServiceTests()
        {
            _sut = new UserService(new CreditLimitProviderFactory(_userCreditService), _clientRepository, _userDataAccess, new UserValidator(_dateTimeService));
        }

        [Fact]
        public void AddUser_ShouldCreatUser_WhenAllParametersAreValid()
        {
            // Arrange
            var client = _fixture.Create<Client>();
            const string firstName = "FirstName";
            const string lastName = "LastName";
            var dateOfBirth = new DateOnly(1980, 1, 1);
            _dateTimeService.DateTimeNow.Returns(new DateTime(2022, 1, 1));
            _clientRepository.GetById(Arg.Is(client.Id)).Returns(client);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(666);

            // Act
            var result = _sut.AddUser(firstName, lastName, "user@example.org", dateOfBirth, client.Id);

            // Assert
            result.Should().BeTrue();
            _userDataAccess.Received(1).AddUser(Arg.Any<User>());
        }

        [Theory]
        [InlineData("", "LastName", "user@emaple.org", 1980)]
        [InlineData("FirstName", "", "user@emaple.org", 1980)]
        [InlineData("FirstName", "LastName", "userexamapleorg", 1980)]
        [InlineData("FirstName", "LastName", "user@emaple.org", 2012)]
        public void AddUser_ShouldNotCreatUser_WhenAllParametersAreInValid(string firstName, string lastName, string email, int yearOfBirth)
        {
            // Arrange
            var client = _fixture.Create<Client>();
            var dateOfBirth = new DateOnly(yearOfBirth, 1, 20);
            _dateTimeService.DateTimeNow.Returns(new DateTime(2022, 1, 10));
            _clientRepository.GetById(Arg.Is(client.Id)).Returns(client);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(666);

            // Act
            var result = _sut.AddUser(firstName, lastName, email, dateOfBirth, client.Id);

            // Assert
            result.Should().BeFalse();
            _userDataAccess.Received(0).AddUser(Arg.Any<User>());
        }

        [Theory]
        [InlineData("RandomClient", true, 666, 666)]
        [InlineData("ImportantClient", true, 666, 1332)]
        [InlineData("VeryImportantClient", false, 0, 0)]
        public void AddUser_ShoulCreatUserWithCorrectCreditLimit_WhenNameIndicatesDifferentClasification(string clientName, bool hasCreditLimit, int initialCreditLimit, int finalCreditLimit)
        {
            // Arrange
            var client = _fixture.Build<Client>()
                .With(c => c.Name, clientName)
                .Create();

            const string firstName = "FirstName";
            const string lastName = "LastName";
            var dateOfBirth = new DateOnly(1980, 10, 10);
            _dateTimeService.DateTimeNow.Returns(new DateTime(2022, 2, 10));
            _clientRepository.GetById(Arg.Is(client.Id)).Returns(client);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(initialCreditLimit);

            // Act
            var result = _sut.AddUser(firstName, lastName, "user@example.org", dateOfBirth, client.Id);

            // Assert
            result.Should().BeTrue();
            _userDataAccess.Received()
                .AddUser(Arg.Is<User>(user => user.HasCreditLimit == hasCreditLimit && user.CreditLimit == finalCreditLimit));
        }

        [Fact]
        public void AddUser_ShouldNotCreatUser_WhenUserHasCreditLimitAndCreditLimitIsLessThan500()
        {
            // Arrange
            var client = _fixture.Create<Client>();
            const string firstName = "FirstName";
            const string lastName = "LastName";
            var dateOfBirth = new DateOnly(1980, 1, 1);
            _dateTimeService.DateTimeNow.Returns(new DateTime(2022, 1, 1));
            _clientRepository.GetById(Arg.Is(client.Id)).Returns(client);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(499);

            // Act
            var result = _sut.AddUser(firstName, lastName, "user@example.org", dateOfBirth, client.Id);

            // Assert
            result.Should().BeFalse();
            _userDataAccess.Received(0).AddUser(Arg.Any<User>());
        }

        [Fact]
        public void Add_ShouldThrowException_WhenClientIdDoesNotExist()
        {
            // Arrange
            const string firstName = "FirstName";
            const string lastName = "LastName";
            var dateOfBirth = new DateOnly(1980, 1, 1);
            _dateTimeService.DateTimeNow.Returns(new DateTime(2022, 1, 1));
            _clientRepository.GetById(Arg.Any<int>()).ReturnsForAnyArgs(_ => null);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(499);

            // Act
            Action action = () => _sut.AddUser(firstName, lastName, "user@example.org", dateOfBirth, -1);

            // Assert
            action.Should().Throw<Exception>();
        }
    }
}