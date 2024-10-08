using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class ChatServer
{
    private TcpListener _listener;
    private List<TcpClient> _clients;
    private List<string> _messageHistory;
    private const int Port = 6000;
    private const int MaxHistory = 100;

    public ChatServer()
    {
        _clients = new List<TcpClient>();
        _messageHistory = new List<string>();
        _listener = new TcpListener(IPAddress.Loopback, Port);
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine("Server started on port " + Port);

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected");

            lock (_clients)
            {
                _clients.Add(client);
            }

            _ = Task.Run(() => HandleClientAsync(client));
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        var stream = client.GetStream();
        var buffer = new byte[1024];

        try
        {
            // Read client name
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string clientName = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Client name: {clientName}");

            // Send message history
            List<string> historyCopy;
            lock (_messageHistory)
            {
                historyCopy = new List<string>(_messageHistory);
            }

            foreach (var message in historyCopy)
            {
                await SendMessageAsync(client, message);
            }

            while (true)
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                lock (_messageHistory)
                {
                    _messageHistory.Add(message);
                    if (_messageHistory.Count > MaxHistory)
                    {
                        _messageHistory.RemoveAt(0);
                    }
                }

                await BroadcastMessageAsync(message, client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            lock (_clients)
            {
                _clients.Remove(client);
            }
            client.Close();
        }
    }

    private async Task SendMessageAsync(TcpClient client, string message)
    {
        var stream = client.GetStream();
        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
    }

    private async Task BroadcastMessageAsync(string message, TcpClient excludeClient)
    {
        List<TcpClient> clientsCopy;

        // Create a copy of the client list
        lock (_clients)
        {
            clientsCopy = new List<TcpClient>(_clients);
        }

        foreach (var client in clientsCopy)
        {
            if (client != excludeClient)
            {
                try
                {
                    await SendMessageAsync(client, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message: {ex.Message}");
                }
            }
        }
    }

    public static async Task Main()
    {
        var server = new ChatServer();
        await server.StartAsync();
    }
}