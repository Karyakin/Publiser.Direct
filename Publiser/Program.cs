// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client;

Task.Run(CreateTask(2000, "error"));
Task.Run(CreateTask(3000, "information"));
Task.Run(CreateTask(4000, "warning"));

Console.ReadKey();


static Func<Task> CreateTask(int timeOutToSleep, string routingKey)
{
    return () =>
    {
        var count = 0;
        do
        {
            int timeToSleep = new Random().Next(1000, timeOutToSleep);
            Thread.Sleep(timeToSleep);
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var chanel = connection.CreateModel();

            chanel.ExchangeDeclare(
                exchange: "direct_logs",
                type: ExchangeType.Direct
            );

            string message = $"message type: {routingKey}";
            var body = Encoding.UTF8.GetBytes(message + "№" + count);

            chanel.BasicPublish(
                exchange: "direct_logs",
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );
            
            Console.WriteLine($"message type: {routingKey}: {count}");
            count++;
        } while (true);
    };
}