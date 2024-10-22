using System;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MainProcessingService
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare a queue
            channel.QueueDeclare(queue: "image_processing_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine("Waiting for messages...");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var filePath = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Received file path: {filePath}");

                // Define the destination path
                string destinationDirectory = Path.Combine("ProcessedImages");
                string destinationPath = Path.Combine(destinationDirectory, Path.GetFileName(filePath));

                // Create the directory if it does not exist
                Directory.CreateDirectory(destinationDirectory);

                try
                {
                    // Simulate processing
                    File.Copy(filePath, destinationPath);
                    Console.WriteLine($"Processed and saved: {destinationPath}");

                    // Optionally, move the file to an archive folder after processing
                    string archiveDirectory = Path.Combine(Path.GetDirectoryName(filePath),"ArchivedImages");
                    Directory.CreateDirectory(archiveDirectory);
                    string archivedPath = Path.Combine(archiveDirectory, Path.GetFileName(filePath));
                    File.Move(filePath, archivedPath);
                    Console.WriteLine($"Moved original file to archive: {archivedPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process the file: {ex.Message}");
                }
            };

            channel.BasicConsume(queue: "image_processing_queue",
                                 autoAck: true,
                                 consumer: consumer);

            Console.ReadLine();
        }
    }
}