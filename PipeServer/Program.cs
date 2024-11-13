using System.IO.Pipes;
using System.Reflection;
using System.Threading;

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
        
        PipeServer5.Run(); // change this to 1-5
    }
}
class PipeServer1
{
    public static void Run()
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out))
        {
            Console.WriteLine("Waiting for client connection...");
            pipeServer.WaitForConnection();

            Console.WriteLine("Client connected.");
            using (StreamWriter writer = new StreamWriter(pipeServer))
            {
                writer.AutoFlush = true;
                writer.WriteLine("Hello, Client!");
            }
        }
    }
}

class PipeServer2
{
    public static void Run()
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut))
        {
            Console.WriteLine("Waiting for client connection...");
            pipeServer.WaitForConnection();
            Console.WriteLine("Client connected.");

            using (StreamWriter writer = new StreamWriter(pipeServer))
            using (StreamReader reader = new StreamReader(pipeServer))
            {
                writer.AutoFlush = true;

                string clientMessage;
                string serverMessage = "Hello, Client!";

                do
                {
                    writer.WriteLine(serverMessage);
                    Console.WriteLine("Sent to client: " + serverMessage);

                    clientMessage = reader.ReadLine();
                    Console.WriteLine("Received from client: " + clientMessage);

                    if (clientMessage != null && !clientMessage.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Write("Enter message to send to client (type 'exit' to end): ");
                        serverMessage = Console.ReadLine();
                    }

                } while (clientMessage != null && !clientMessage.Equals("exit", StringComparison.OrdinalIgnoreCase));
            }
        }

        Console.WriteLine("Server is done. Press any key to exit.");
        Console.ReadKey();
    }
}

class PipeServer3
{
    public static void Run()
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut))
        {
            Console.WriteLine("Waiting for client connection...");
            pipeServer.WaitForConnection();

            Console.WriteLine("Client connected.");
            using (StreamReader reader = new StreamReader(pipeServer))
            using (StreamWriter writer = new StreamWriter(pipeServer))
            {
                writer.AutoFlush = true;

                while (true)
                {
                    string message = reader.ReadLine();
                    if (message == null) break;

                    Console.WriteLine("Received from client: " + message);

                    writer.WriteLine("Message received");
                }
            }
        }
    }
}

class PipeServer4
{
    public static void Run()
    {
        string rootDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\"));
        string outputFilePath = Path.Combine(rootDirectory, "received_file.txt");

        try
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("filepipe", PipeDirection.InOut))
            {
                Console.WriteLine("Waiting for client connection...");
                pipeServer.WaitForConnection();
                Console.WriteLine("Client connected.");

                using (StreamReader reader = new StreamReader(pipeServer))
                using (StreamWriter writer = new StreamWriter(pipeServer))
                {
                    writer.AutoFlush = true;

                    using (StreamWriter fileWriter = new StreamWriter(outputFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Console.WriteLine("Received: " + line);
                            fileWriter.WriteLine(line);

                            writer.WriteLine("Line received");
                        }
                    }
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("An error occurred with the pipe: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("A general error occurred: " + ex.Message);
        }
    }
}

class PipeServer5
{
    public static void Run()
    {
        Console.WriteLine("Pipe Server started...");

        while (true)
        {
            try
            {
                NamedPipeServerStream pipeServer = new NamedPipeServerStream("multiPipe", PipeDirection.InOut, 10, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                Console.WriteLine("Waiting for a client to connect...");

                pipeServer.WaitForConnection();
                Console.WriteLine("Client connected.");

                Thread clientThread = new Thread(() => HandleClient(pipeServer));
                clientThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error with server: " + ex.Message);
            }
        }
    }

    private static void HandleClient(NamedPipeServerStream pipeServer)
    {
        try
        {
            using (pipeServer)
            {
                StreamReader reader = new StreamReader(pipeServer);
                StreamWriter writer = new StreamWriter(pipeServer) { AutoFlush = true };

                string message;
                while ((message = reader.ReadLine()) != null)
                {
                    Console.WriteLine("Received from client: " + message);

                    writer.WriteLine("Message received: " + message);
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Error with client connection: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("General error: " + ex.Message);
        }
    }
}