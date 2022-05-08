using LegacyApp.DataAccess;
using LegacyApp.Models;
using LegacyApp.Repositories;
using LegacyApp.Services;

namespace LegacyApp;

public class UserService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IClientRepository _clientRepository;
    private readonly IUserCreditService _userCreditService;
    private readonly IUserDataAccess _userDataAccess;

    public UserService() : this(
        new DateTimeService(),
        new ClientRepository(),
        new UserCreditServiceClient(),
        new UserDataAccessProxy())
    {
    }

    public UserService(
        IDateTimeService dateTimeService,
        IClientRepository clientRepository,
        IUserCreditService userCreditService,
        IUserDataAccess userDataAccess)
    {
        _dateTimeService = dateTimeService;
        _clientRepository = clientRepository;
        _userCreditService = userCreditService;
        _userDataAccess = userDataAccess;
    }

    public bool AddUser(string firstName, string lastName, string email, DateOnly dateOfBirth, int clientId)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            return false;
        }

        if (!email.Contains('@') && !email.Contains('.'))
        {
            return false;
        }

        var now = _dateTimeService.DateTimeNow;
        var age = now.Year - dateOfBirth.Year;
        if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
        {
            age--;
        }

        if (age < 21)
        {
            return false;
        }

        var client = _clientRepository.GetById(clientId);

        if (client is null)
        {
            throw new Exception();
        }

        var user = new User
        {
            Client = client,
            DateOfBirth = dateOfBirth,
            EmailAddress = email,
            FirstName = firstName,
            LastName = lastName
        };

        if (client.Name == "VeryImportantClient")
        {
            // Skip credit check
            user.HasCreditLimit = false;
        }
        else if (client.Name == "ImportantClient")
        {
            // Do credit check and double credit limit
            user.HasCreditLimit = true;
            var creditLimit = _userCreditService.GetCreditLimit(user.FirstName, user.LastName, user.DateOfBirth);
            creditLimit = creditLimit * 2;
            user.CreditLimit = creditLimit;
        }
        else
        {
            // Do credit check
            user.HasCreditLimit = true;
            var creditLimit = _userCreditService.GetCreditLimit(user.FirstName, user.LastName, user.DateOfBirth);
            user.CreditLimit = creditLimit;
        }

        if (user.HasCreditLimit && user.CreditLimit < 500)
        {
            return false;
        }

        _userDataAccess.AddUser(user);

        return true;
    }
}