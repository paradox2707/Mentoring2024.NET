using RabbitMQ.Client;

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

            string folderPath = @"C:\DATA\.NET Mentoring Program Intermediate [UA Q4 2024]\Mentoring2024.NET - Repo\03.MessageQueues\SourceFolderForFiles"; // Change to your folder path

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
