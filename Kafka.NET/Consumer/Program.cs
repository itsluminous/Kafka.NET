using Confluent.Kafka;

namespace Consumer;

internal static class Program
{
    private static readonly string BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_SERVER") ?? "localhost:9092";
    private static readonly string Topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC") ?? "MyTopic";
    private static readonly string ConsumerGroupId = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_GROUP") ?? "MyConsumerGroup";
    

    static void Main()
    {
        // Set up Kafka consumer configuration
        var config = new ConsumerConfig
        {
            BootstrapServers = BootstrapServers,
            GroupId = ConsumerGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        // Create a Kafka consumer
        using var consumer = new ConsumerBuilder<Ignore, byte[]>(config).Build();
        // Subscribe to the Kafka topic
        consumer.Subscribe(Topic);

        // Poll for new messages from the Kafka topic
        while (true)
        {
            try
            {
                var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
                if (consumeResult == null)
                    continue;

                // Convert the consumed bytes to your Protobuf message
                var message = Employee.Parser.ParseFrom(consumeResult.Message.Value);

                // Print the message
                Console.WriteLine($"Received message: {message}");

            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occurred: {e.Error.Reason}");
            }
        }
    }
}