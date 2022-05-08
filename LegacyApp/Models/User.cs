namespace LegacyApp.Models;

public class User
{
    public bool HasCreditLimit { get; internal set; }

    public string FirstName { get; internal set; } = default!;

    public string LastName { get; internal set; } = default!;

    public string EmailAddress { get; internal set; } = default!;

    public DateOnly DateOfBirth { get; internal set; }

    public int CreditLimit { get; set; }

    public Client Client { get; set; } = default!;
}