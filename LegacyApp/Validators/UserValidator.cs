using LegacyApp.Models;
using LegacyApp.Services;

namespace LegacyApp.Validators;

public class UserValidator
{
    private readonly IDateTimeService _dateTimeService;

    public UserValidator(IDateTimeService dateTimeService)
    {
        _dateTimeService = dateTimeService;
    }

    public bool HasCreditLimitAndLimitIsLessThan500(User user)
    {
        return user.HasCreditLimit && user.CreditLimit < 500;
    }

    public bool HasValidEmail(string email)
    {
        return email.Contains('@') || email.Contains('.');
    }

    public bool HasValidFullName(string firstName, string lastName)
    {
        return !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName);
    }

    public bool IsAtLeast21YearsOld(DateOnly dateOfBirth)
    {
        var now = _dateTimeService.DateTimeNow;
        var age = now.Year - dateOfBirth.Year;
        if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
        {
            age--;
        }

        return age >= 21;
    }
}