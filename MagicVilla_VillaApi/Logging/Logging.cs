namespace MagicVilla_VillaApi.Logging
{
    public class Logging : ILogging
    {
        public void Log(string message, string type)
        {
            switch (type)
            {
                case "error":
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR - " + message);
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case "warning":
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Warning - " + message);
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                default:
                    Console.WriteLine("Info - " + message);
                    break;
            };

        }
    }
}
