namespace LegacyApp.CreditLimitProviders;

public interface ICreditLimitProviderFactory
{
    ICreditLimitProvider GetProviderByClientName(string clientName);
}