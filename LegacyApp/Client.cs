namespace LegacyApp;

public class Client
{
    public int Id { get; internal set; }

    public string Name { get; internal set; } = default!;

    public ClientStatus ClientStatus { get; internal set; }
}