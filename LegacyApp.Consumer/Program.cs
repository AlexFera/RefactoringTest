namespace LegacyApp.Consumer;

static class Program
{
    static void Main(string[] args)
    {
        ProveAddUser(args);
    }

    public static void ProveAddUser(string[] args)
    {
        /*
         * DO NOT CHANGE THIS FILE AT ALL
        */

        var userService = new UserService();
        var addResult = userService.AddUser("Alex", "Fera", "alexandrufera@gmail.com", new DateOnly(1991, 1, 1), 4);
        Console.WriteLine("Adding Alex Fera was " + (addResult ? "successful" : "unsuccessful"));
    }
}