using LegacyApp.CreditLimitProviders;
using LegacyApp.DataAccess;
using LegacyApp.Models;
using LegacyApp.Repositories;
using LegacyApp.Services;
using LegacyApp.Validators;

namespace LegacyApp;

public class UserService
{
    private readonly IClientRepository _clientRepository;
    private readonly IUserDataAccess _userDataAccess;
    private readonly ICreditLimitProviderFactory _creditLimitProviderFactory;
    private readonly UserValidator _userValidator;

    public UserService() : this(
        new CreditLimitProviderFactory(new UserCreditServiceClient()),
        new ClientRepository(),
        new UserDataAccessProxy(),
        new UserValidator(new DateTimeService()))
    {
    }

    public UserService(
        ICreditLimitProviderFactory creditLimitProviderFactory,
        IClientRepository clientRepository,
        IUserDataAccess userDataAccess,
        UserValidator userValidator)
    {
        _creditLimitProviderFactory = creditLimitProviderFactory;
        _clientRepository = clientRepository;
        _userDataAccess = userDataAccess;
        _userValidator = userValidator;
    }

    public bool AddUser(string firstName, string lastName, string email, DateOnly dateOfBirth, int clientId)
    {
        if (!UserProvidedDataIsValid(firstName, lastName, email, dateOfBirth))
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

        ApplyCreditLimits(client, user);

        if (_userValidator.HasCreditLimitAndLimitIsLessThan500(user))
        {
            return false;
        }

        _userDataAccess.AddUser(user);

        return true;
    }

    private void ApplyCreditLimits(Client client, User user)
    {
        var creditLimitProvider = _creditLimitProviderFactory.GetProviderByClientName(client.Name);
        var (hasCreditLimit, creditLimit) = creditLimitProvider.GetCreditLimits(user);
        user.HasCreditLimit = hasCreditLimit;
        user.CreditLimit = creditLimit;
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