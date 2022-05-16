namespace bot;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== PRESS `q` AT ANY TIME TO STOP THIS ===");

        var bot = new GemBot();
        bot.Start();

        while (true)
        {
            var input = Console.ReadKey();

            if (input.KeyChar == 'q')
            {
                break;
            }
        }
    }
}
