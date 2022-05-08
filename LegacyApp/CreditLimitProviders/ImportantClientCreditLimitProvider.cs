using LegacyApp.Models;
using LegacyApp.Services;

namespace LegacyApp.CreditLimitProviders;

public class ImportantClientCreditLimitProvider : ICreditLimitProvider
{
    private readonly IUserCreditService _userCreditService;

    public ImportantClientCreditLimitProvider(IUserCreditService userCreditService)
    {
        _userCreditService = userCreditService;
    }

    public string NameRequirement => "ImportantClient";

    public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user)
    {
        var creditLimit = _userCreditService.GetCreditLimit(user.FirstName, user.LastName, user.DateOfBirth);
        creditLimit *= 2;

        return (true, creditLimit);
    }
}