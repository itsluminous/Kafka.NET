using Confluent.Kafka;
using Google.Protobuf;

namespace Producer;

internal static class Program
{
    private static readonly string BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_SERVER") ?? "localhost:9092";
    private static readonly string Topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC") ?? "MyTopic";
    private static readonly string InputFile = Environment.GetEnvironmentVariable("INPUT_FILE") ?? "/tmp/input.txt";

    private static void Main()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = BootstrapServers
        };

        // Create a Kafka producer
        using var producer = new ProducerBuilder<string, byte[]>(config).Build();
        // Monitor the file for changes
        using var fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(InputFile)!);
        fileSystemWatcher.Filter = Path.GetFileName(InputFile);
        fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;

        // Define the event handler for file change events
        fileSystemWatcher.Changed += (sender, e) =>
        {
            // Read the changed data from the file
            var changedData = File.ReadAllText(e.FullPath);

            // Split the file data into fields (assuming data format is delimited)
            var fields = changedData.Split('|');

            // Create an instance of the Protobuf message and populate its fields
            var employee = new Employee
            {
                Id = fields[0],
                Name = fields[1],
                Department = fields[2],
                Salary = float.Parse(fields[3])
            };

            // Publish the changed data to the Kafka topic
            producer.Produce(Topic, new Message<string, byte[]> { Key = employee.Id, Value = employee.ToByteArray() },
                deliveryReport =>
                {
                    Console.WriteLine(deliveryReport.Error != null && deliveryReport.Error.IsError
                        ? $"Failed to publish message: {deliveryReport.Error.Reason}"
                        : $"Message published: {deliveryReport.Offset}");
                });
        };

        // Start monitoring the file
        fileSystemWatcher.EnableRaisingEvents = true;

        // Keep the application running
        Console.WriteLine("Press Ctrl+C to exit.");
        ManualResetEvent waitHandle = new ManualResetEvent(false);
        Console.CancelKeyPress += (sender, e) =>
        {
            waitHandle.Set();
            e.Cancel = true;
        };
        waitHandle.WaitOne();
    }
}
