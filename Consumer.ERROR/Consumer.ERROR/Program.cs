using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};
using var connection = factory.CreateConnection();
using var chanel = connection.CreateModel();

chanel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

var queueName = chanel.QueueDeclare().QueueName;
chanel.QueueBind(
    queue: queueName,
    exchange: "direct_logs",
    routingKey: "error"
    );
    
var consumer = new EventingBasicConsumer(chanel);
    
consumer.Received += (sender, e) =>
{
    var body = e.Body;
    var message = Encoding.UTF8.GetString(body.ToArray());
    Console.WriteLine($"Received message {message}");
};
//Связываем конзьюмера с очередью
chanel.BasicConsume(
    queue: queueName,
    autoAck: true,
    consumer: consumer
);
Console.ReadKey();