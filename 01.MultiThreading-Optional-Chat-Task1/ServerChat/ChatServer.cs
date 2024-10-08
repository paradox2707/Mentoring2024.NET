using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
        IPAddress ipAddress = IPAddress.Loopback;
        _listener = new TcpListener(ipAddress, Port);
    }

    public void Start()
    {
        //_listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _listener.Start();
        Console.WriteLine("Server started on port " + Port);

        while (true)
        {
            var client = _listener.AcceptTcpClient();
            Console.WriteLine("Client connected");

            lock (_clients)
            {
                _clients.Add(client);
            }

            var clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    private void HandleClient(object clientObj)
    {
        var client = (TcpClient)clientObj;
        var stream = client.GetStream();
        var buffer = new byte[1024];

        try
        {
            // Read client name
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string clientName = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Client name: {clientName}");

            // Send message history
            lock (_messageHistory)
            {
                foreach (var message in _messageHistory)
                {
                    SendMessage(client, message);
                }
            }

            while (true)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
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

                BroadcastMessage(message, client);
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

    private void SendMessage(TcpClient client, string message)
    {
        var stream = client.GetStream();
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    private void BroadcastMessage(string message, TcpClient excludeClient)
    {
        lock (_clients)
        {
            foreach (var client in _clients)
            {
                if (client != excludeClient)
                {
                    SendMessage(client, message);
                }
            }
        }
    }

    public static void Main()
    {
        var server = new ChatServer();
        server.Start();
    }
}