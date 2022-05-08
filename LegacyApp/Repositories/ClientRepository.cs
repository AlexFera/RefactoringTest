using LegacyApp.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LegacyApp.Repositories;

public class ClientRepository
{
    public Client? GetById(int id)
    {
        Client? client = null;
        var connectionString = ConfigurationManager.ConnectionStrings["appDatabase"].ConnectionString;
        using var connection = new SqlConnection(connectionString);
        var command = new SqlCommand
        {
            Connection = connection,
            CommandType = CommandType.StoredProcedure,
            CommandText = "uspGetClientById"
        };

        var clientIdParameter = new SqlParameter("@clientId", SqlDbType.Int) { Value = id };
        command.Parameters.Add(clientIdParameter);

        connection.Open();
        var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
        while (reader.Read())
        {
            {
                client = new Client
                {
                    Id = int.Parse((string)reader!["ClientId"]),
                    Name = (string)reader!["Name"],
                    ClientStatus = (ClientStatus)int.Parse((string)reader!["ClientStatus"])
                };
            }
        }

        return client;
    }
}
