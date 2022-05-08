using System.ServiceModel;

namespace LegacyApp.Services;

[ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
public interface IUserCreditService
{
    [OperationContract]
    int GetCreditLimit(string firstName, string lastName, DateOnly dateOfBirth);
}

public partial class UserCreditServiceClient : ClientBase<IUserCreditService>, IUserCreditService
{
    public UserCreditServiceClient()
    { }

    public UserCreditServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
    { }

    public UserCreditServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
    { }

    public UserCreditServiceClient(string endpointConfigurationName,
        EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
    { }

    public UserCreditServiceClient(System.ServiceModel.Channels.Binding binding,
        EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
    { }

    public int GetCreditLimit(string firstName, string lastName, DateOnly dateOfBirth)
    {
        return Channel.GetCreditLimit(firstName, lastName, dateOfBirth);
    }
}