using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class ChatClient
{
    private const string ServerAddress = "127.0.0.1"; // Localhost
    private const int Port = 6000;
    private static readonly string[] Messages = { "Hello!", "How are you?", "Goodbye!" };

    public static void Main()
    {
        while (true)
        {
            try
            {
                using (var client = new TcpClient(ServerAddress, Port))
                {
                    Console.WriteLine("Connected to server.");
                    var stream = client.GetStream();

                    // Send client name
                    string clientName = "Client_" + Guid.NewGuid();
                    byte[] nameData = Encoding.UTF8.GetBytes(clientName);
                    stream.Write(nameData, 0, nameData.Length);

                    // Start receiving messages
                    var receiveTask = Task.Run(() => ReceiveMessages(stream));

                    // Send messages
                    Random random = new Random();
                    foreach (var message in Messages)
                    {
                        Thread.Sleep(random.Next(500, 2000)); // Random delay
                        byte[] messageData = Encoding.UTF8.GetBytes(message);
                        stream.Write(messageData, 0, messageData.Length);
                        Console.WriteLine($"Sent: {message}");
                    }

                    // Wait for receiving to finish
                    receiveTask.Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to reconnect or Ctrl+C to exit.");
            Console.ReadKey();
        }
    }

    private static void ReceiveMessages(NetworkStream stream)
    {
        var buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");
            }
        }
        catch (ObjectDisposedException)
        {
            Console.WriteLine("The connection was closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Receiving error: {ex.Message}");
        }
    }
}