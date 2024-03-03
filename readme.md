# Install and start kafka in local
1. ``brew install confluentinc/tap/cli``
2. ``confluent local kafka start``

# Configure kafka
1. Create a topic : ``confluent local kafka topic create learning``
2. foo

# Start producer and consumer projects
1. Build producer image : ``docker build -t producer.net Kafka.NET/Producer``
2. Build consumer image : ``docker build -t consumer.net Kafka.NET/Consumer``
3. Start producer container : ``docker run -d -v ./Kafka.NET:/Config producer.net``
4. Start consumer container : ``docker run -d -v ./Kafka.NET:/Config consumer.net``