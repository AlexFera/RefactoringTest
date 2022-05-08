using LegacyApp.Models;

namespace LegacyApp.CreditLimitProviders;

public class VeryImportantClientCreditLimitProvider : ICreditLimitProvider
{
    public string NameRequirement => "VeryImportantClient";

    public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user)
    {
        return (false, 0);
    }
}