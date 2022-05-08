using LegacyApp.Models;

namespace LegacyApp.CreditLimitProviders;

public interface ICreditLimitProvider
{
    (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user);

    public string NameRequirement { get; }
}