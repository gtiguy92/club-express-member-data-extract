using System;

namespace uaip_member_extract
{
    class Program
    {
        static void Main(string[] args)
        {
            // Validate that the correct number of arguments is provided
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: <application> <Url> <Username> <Password>");
            return;
        }

        // Extract arguments
        string url = args[0];
        string username = args[1];
        string password = args[2];

        // Create an instance of UAIPMembers with the provided arguments
        UAIPMembers members = new UAIPMembers(url, username, password);

        // Call the Test method
        members.Test();
        }
    }
}
