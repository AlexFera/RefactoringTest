using LegacyApp.Models;
using LegacyApp.Services;

namespace LegacyApp.CreditLimitProviders
{
    public class DefaultCreditLimitProvider : ICreditLimitProvider
    {
        private readonly IUserCreditService _userCreditService;

        public DefaultCreditLimitProvider(IUserCreditService userCreditService)
        {
            _userCreditService = userCreditService;
        }

        public string NameRequirement => string.Empty;

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user)
        {
            var creditLimit = _userCreditService.GetCreditLimit(user.FirstName, user.LastName, user.DateOfBirth);

            return (true, creditLimit);
        }
    }
}
