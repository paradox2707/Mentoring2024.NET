using RabbitMQ.Client;
using System.Configuration;

namespace DataCaptureService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Set up RabbitMQ connection and channel
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare a queue
            channel.QueueDeclare(queue: "image_processing_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string folderPath = ConfigurationManager.AppSettings["SourceFolderPath"];

            Console.WriteLine("Monitoring folder for new PDF files...");

            while (true)
            {
                var files = Directory.GetFiles(folderPath, "*.pdf");
                foreach (var file in files)
                {
                    // Send file path to RabbitMQ
                    var body = System.Text.Encoding.UTF8.GetBytes(file);
                    channel.BasicPublish(exchange: "",
                                         routingKey: "image_processing_queue",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine($"Sent: {file}");
                }

                Thread.Sleep(5000); // Check for new files every 5 seconds
            }
        
        }
    }
}
