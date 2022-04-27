namespace LegacyApp;

public class UserService
{
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

        var now = DateTime.Now;
        var age = now.Year - dateOfBirth.Year;
        if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
        {
            age--;
        }

        if (age < 21)
        {
            return false;
        }

        var clientRepository = new ClientRepository();
        var client = clientRepository.GetById(clientId);

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
            using var userCreditService = new UserCreditServiceClient();
            var creditLimit = userCreditService.GetCreditLimit(user.FirstName, user.LastName, user.DateOfBirth);
            creditLimit = creditLimit * 2;
            user.CreditLimit = creditLimit;
        }
        else
        {
            // Do credit check
            user.HasCreditLimit = true;
            using var userCreditService = new UserCreditServiceClient();
            var creditLimit = userCreditService.GetCreditLimit(user.FirstName, user.LastName, user.DateOfBirth);
            user.CreditLimit = creditLimit;
        }

        if (user.HasCreditLimit && user.CreditLimit < 500)
        {
            return false;
        }

        UserDataAccess.AddUser(user);

        return true;
    }
}