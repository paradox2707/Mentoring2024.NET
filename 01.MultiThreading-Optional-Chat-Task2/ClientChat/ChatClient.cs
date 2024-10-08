using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class ChatClient
{
    private const string ServerAddress = "127.0.0.1";
    private const int Port = 6000;
    private static readonly string[] Messages = { "Hello!", "How are you?", "Goodbye!" };

    public static async Task Main()
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
                    await stream.WriteAsync(nameData, 0, nameData.Length);

                    // Start receiving messages
                    var receiveTask = ReceiveMessagesAsync(stream);

                    // Send messages
                    Random random = new Random();
                    foreach (var message in Messages)
                    {
                        byte[] messageData = Encoding.UTF8.GetBytes(message);
                        await stream.WriteAsync(messageData, 0, messageData.Length);
                        Console.WriteLine($"Sent: {message}");
                        await Task.Delay(random.Next(500, 2000)); // Random delay
                    }

                    // Wait for receiving to finish
                    await receiveTask;
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

    private static async Task ReceiveMessagesAsync(NetworkStream stream)
    {
        var buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Receiving error: {ex.Message}");
        }
    }
}