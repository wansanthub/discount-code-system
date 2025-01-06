namespace DiscountCodeSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string serverUrl = "http://localhost:5225";
            var client = new Client(serverUrl);

            Console.WriteLine("Starting client...");
            await client.StartAsync();

            while (true)
            {
                Console.WriteLine("\nAvailable commands:");
                Console.WriteLine("1. Generate Codes");
                Console.WriteLine("2. Use Code");
                Console.WriteLine("3. Exit");
                Console.Write("Enter your choice: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter the number of codes to generate (max 2000): ");
                        if (ushort.TryParse(Console.ReadLine(), out var count) && count <= 2000)
                        {
                            Console.Write("Enter the code length (7 or 8): ");
                            if (byte.TryParse(Console.ReadLine(), out var length) && (length == 7 || length == 8))
                            {
                                await client.GenerateCodes(count, length);
                            }
                            else
                            {
                                Console.WriteLine("Invalid code length.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid number of codes.");
                        }
                        break;

                    case "2":
                        Console.Write("Enter the code to use: ");
                        var code = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(code))
                        {
                            await client.UseCode(code);
                        }
                        else
                        {
                            Console.WriteLine("Invalid code.");
                        }
                        break;

                    case "3":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}