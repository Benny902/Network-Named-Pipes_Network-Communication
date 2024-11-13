using System.IO.Pipes;
using System.Reflection;

class Program
{
    static void Main()
    { // a second client for using fifth method - Multiple Clients Communication
        PipeClient5.Run();
    }
}

class PipeClient5
{
    public static void Run()
    {
        try
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "multiPipe", PipeDirection.InOut))
            {
                Console.WriteLine("Connecting to the server...");
                pipeClient.Connect();
                Console.WriteLine("Connected to the server.");

                using (StreamReader reader = new StreamReader(pipeClient))
                using (StreamWriter writer = new StreamWriter(pipeClient) { AutoFlush = true })
                {
                    string message;
                    while (true)
                    {
                        Console.Write("Enter message for server (or type 'exit' to quit): ");
                        message = Console.ReadLine();

                        if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        writer.WriteLine(message);
                        string response = reader.ReadLine();
                        Console.WriteLine("Received from server: " + response);
                    }
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Error with pipe communication: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}