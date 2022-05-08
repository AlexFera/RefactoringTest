using LegacyApp.DataAccess;
using LegacyApp.Models;
using LegacyApp.Repositories;
using LegacyApp.Services;
using LegacyApp.Validators;

namespace LegacyApp;

public class UserService
{
    private readonly IClientRepository _clientRepository;
    private readonly IUserCreditService _userCreditService;
    private readonly IUserDataAccess _userDataAccess;
    private readonly UserValidator _userValidator;

    public UserService() : this(
        new ClientRepository(),
        new UserCreditServiceClient(),
        new UserDataAccessProxy(),
        new UserValidator(new DateTimeService()))
    {
    }

    public UserService(
        IClientRepository clientRepository,
        IUserCreditService userCreditService,
        IUserDataAccess userDataAccess,
        UserValidator userValidator)
    {
        _clientRepository = clientRepository;
        _userCreditService = userCreditService;
        _userDataAccess = userDataAccess;
        _userValidator = userValidator;
    }

    public bool AddUser(string firstName, string lastName, string email, DateOnly dateOfBirth, int clientId)
    {
        if (!UserProvidedDataIsValid(firstName, lastName,  email, dateOfBirth))
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

        if (_userValidator.HasCreditLimitAndLimitIsLessThan500(user))
        {
            return false;
        }

        _userDataAccess.AddUser(user);

        return true;
    }

    private bool UserProvidedDataIsValid(string firstName, string lastName, string email, DateOnly dateOfBirth)
    {
        if (!_userValidator.HasValidFullName(firstName, lastName))
        {
            return false;
        }

        if (!_userValidator.HasValidEmail(email))
        {
            return false;
        }

        if (!_userValidator.IsAtLeast21YearsOld(dateOfBirth))
        {
            return false;
        }

        return true;
    }
}