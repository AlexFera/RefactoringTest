using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LegacyApp;

public static class UserDataAccess
{
    public static void AddUser(User user)
    {
        var connectionString = ConfigurationManager.ConnectionStrings["appDatabase"].ConnectionString;

        using var connection = new SqlConnection(connectionString);

        var command = new SqlCommand
        {
            Connection = connection,
            CommandType = CommandType.StoredProcedure,
            CommandText = "uspAddUser"
        };

        var firstNameParameter = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50) { Value = user.FirstName };
        command.Parameters.Add(firstNameParameter);
        var lastNameParameter = new SqlParameter("@LastName", SqlDbType.VarChar, 50) { Value = user.LastName };
        command.Parameters.Add(lastNameParameter);
        var dateOfBirthParameter = new SqlParameter("@DateOfBirth", SqlDbType.Date) { Value = user.DateOfBirth };
        command.Parameters.Add(dateOfBirthParameter);
        var emailParameter = new SqlParameter("@EmailAddress", SqlDbType.NVarChar, 50) { Value = user.EmailAddress };
        command.Parameters.Add(emailParameter);
        var hasCreditLimitParameter = new SqlParameter("@HasCreditLimit", SqlDbType.Bit) { Value = user.HasCreditLimit };
        command.Parameters.Add(hasCreditLimitParameter);
        var creditLimitParameter = new SqlParameter("@CreditLimit", SqlDbType.Int) { Value = user.CreditLimit };
        command.Parameters.Add(creditLimitParameter);
        var clientIdParameter = new SqlParameter("@ClientId", SqlDbType.Int) { Value = user.Client.Id };
        command.Parameters.Add(clientIdParameter);

        connection.Open();
        command.ExecuteNonQuery();
    }
}