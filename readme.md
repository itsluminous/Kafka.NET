# Go to project directory
1. ``cd Kafka.NET``

# Start kafka in local
1. ``docker-compose up -d``

# Start producer and consumer projects
1. ``dotnet build Contract/Contract.csproj``
2. ``dotnet build Producer/Producer.csproj``
3. ``dotnet build Consumer/Consumer.csproj``
4. ``dotnet run --project Producer/Producer.csproj``
5. ``dotnet run --project Consumer/Consumer.csproj``

> Make any change in ~/tmp/input.txt to see the message getting published.  
> Eg. Put this in file : ``1|BalaJi|Engineer|5000``

<!-- # Start producer and consumer projects
1. Build producer image : ``docker build -t producer.net Kafka.NET/Producer``
2. Build consumer image : ``docker build -t consumer.net Kafka.NET/Consumer``
3. Start producer container : ``docker run -d -v ./Kafka.NET:/Config producer.net``
4. Start consumer container : ``docker run -d -v ./Kafka.NET:/Config consumer.net`` -->


# Generate new class for proto change
1. ``cd Kafka.NET/Contract``
2. ``protoc --csharp_out=. Employee.proto``