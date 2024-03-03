using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Consumer;

public static class Consumer {
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

        configuration["group.id"] = "kafka-dotnet-getting-started";
        configuration["auto.offset.reset"] = "earliest";

        const string topic = "purchases";

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };

        using var consumer = new ConsumerBuilder<string, string>(
            configuration.AsEnumerable()).Build();
        consumer.Subscribe(topic);
        try {
            while (true) {
                var cr = consumer.Consume(cts.Token);
                Console.WriteLine($"Consumed event from topic {topic}: key = {cr.Message.Key,-10} value = {cr.Message.Value}");
            }
        }
        catch (OperationCanceledException) {
            // Ctrl-C was pressed.
        }
        finally{
            consumer.Close();
        }
    }
}