using System.IO.Pipes;
using System.Reflection;

class Program
{
    static void Main()
    { 
        /*
          1 - Basic Named Pipe Server and Client
          2 - Bidirectional Communication
          3 - Message Exchange with User Input
          4 - File Transfer over Named Pipes
          5 - Multiple Clients Communication
       */

        PipeClient5.Run(); // change this to 1-5
    }
}

class PipeClient1
{
    public static void Run()
    {
        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.In))
        {
            Console.WriteLine("Connecting to server...");
            pipeClient.Connect();

            using (StreamReader reader = new StreamReader(pipeClient))
            {
                string message = reader.ReadLine();
                Console.WriteLine("Received from server: " + message);
            }
        }
    }
}

class PipeClient2
{
    public static void Run()
    {
        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut))
        {
            Console.WriteLine("Connecting to server...");
            pipeClient.Connect();

            using (StreamReader reader = new StreamReader(pipeClient))
            using (StreamWriter writer = new StreamWriter(pipeClient))
            {
                writer.AutoFlush = true;

                string serverMessage;
                string clientMessage = "";

                do
                {
                    serverMessage = reader.ReadLine();
                    Console.WriteLine("Received from server: " + serverMessage);

                    if (serverMessage != null && !serverMessage.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Write("Enter message to send to server (type 'exit' to end): ");
                        clientMessage = Console.ReadLine();
                        writer.WriteLine(clientMessage);
                        Console.WriteLine("Sent to server: " + clientMessage);
                    }

                } while (serverMessage != null && !serverMessage.Equals("exit", StringComparison.OrdinalIgnoreCase));
            }
        }
        Console.WriteLine("Client is done. Press any key to exit.");
        Console.ReadKey();
    }
}

class PipeClient3
{
    public static void Run()
    {
        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut))
        {
            Console.WriteLine("Connecting to server...");
            pipeClient.Connect();

            using (StreamReader reader = new StreamReader(pipeClient))
            using (StreamWriter writer = new StreamWriter(pipeClient))
            {
                writer.AutoFlush = true;

                while (true)
                {
                    Console.Write("Enter message (or type 'exit' to quit): ");
                    string message = Console.ReadLine();

                    if (message.ToLower() == "exit")
                    {
                        break;
                    }

                    writer.WriteLine(message);

                    string response = reader.ReadLine();
                    Console.WriteLine("Server response: " + response);
                }
            }
        }
    }
}

class PipeClient4
{
    public static void Run()
    {
        string rootDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\"));
        string inputFilePath = Path.Combine(rootDirectory, "sample_file.txt");  // File to send from client root

        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine("Error: The file 'sample_file.txt' was not found at: " + inputFilePath);
            return;
        }

        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "filepipe", PipeDirection.InOut))
        {
            Console.WriteLine("Connecting to server...");
            pipeClient.Connect();
            Console.WriteLine("Connected to server.");

            using (StreamReader reader = new StreamReader(pipeClient))
            using (StreamWriter writer = new StreamWriter(pipeClient))
            {
                writer.AutoFlush = true;

                // Open the file to be sent from the client root directory
                using (StreamReader fileReader = new StreamReader(inputFilePath))
                {
                    string line;
                    while ((line = fileReader.ReadLine()) != null)
                    {
                        // Send each line to the server
                        writer.WriteLine(line);

                        // Wait for acknowledgment from the server
                        string response = reader.ReadLine();
                        Console.WriteLine("Server response: " + response);
                    }
                }
            }
        }
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