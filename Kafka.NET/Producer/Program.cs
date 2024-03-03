using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Producer;

public static class Producer
{
    private const string DefaultConfigPath = "/Config/kafka.properties";

    private static void Main(string[] args)
    {
        var configPath = DefaultConfigPath;
        if (args.Length == 1)
            configPath = args[0];
        else if(!File.Exists(configPath))
            throw new ArgumentNullException("Please provide the configuration file path as a command line argument or put it in valid path");
            

        IConfiguration configuration = new ConfigurationBuilder()
            .AddIniFile(configPath)
            .Build();

        const string topic = "purchases";

        string[] users = { "eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther" };
        string[] items = { "book", "alarm clock", "t-shirts", "gift card", "batteries" };

        using var producer = new ProducerBuilder<string, string>(configuration.AsEnumerable()).Build();
        var numProduced = 0;
        var rnd = new Random();
        const int numMessages = 10;
        for (var i = 0; i < numMessages; ++i)
        {
            var user = users[rnd.Next(users.Length)];
            var item = items[rnd.Next(items.Length)];

            producer.Produce(topic, new Message<string, string> { Key = user, Value = item },
                (deliveryReport) =>
                {
                    if (deliveryReport.Error.Code != ErrorCode.NoError) {
                        Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                    }
                    else {
                        Console.WriteLine($"Produced event to topic {topic}: key = {user,-10} value = {item}");
                        numProduced += 1;
                    }
                });
        }

        producer.Flush(TimeSpan.FromSeconds(10));
        Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
    }
}