using RabbitMQ.Client;

namespace ProviderProject;

public class TopologyMaker
{
    private readonly IConnection _connection;
    private readonly RabbitMqConfig _rabbitMqConfig;

    public TopologyMaker(RabbitMqConfig configs)
    {
        _rabbitMqConfig = configs;

        var connectionFactory = new ConnectionFactory
        {
            UserName = _rabbitMqConfig.Username,
            Password = _rabbitMqConfig.Password,
            AutomaticRecoveryEnabled = _rabbitMqConfig.AutomaticRecoveryEnabled,
            ClientProvidedName = "SetupDecisionRunner"
        };

        var endpoints = ConvertHostNames(configs.HostName)
            .Select(item => new AmqpTcpEndpoint(item.Name, item.Port))
            .ToList();

        _connection = connectionFactory.CreateConnection(endpoints);
        AppDomain.CurrentDomain.ProcessExit += Dispose;
    }

    void Dispose(object sender, EventArgs e)
    {
        _connection.Dispose();
    }

    public void Make()
    {
        var channel = _connection.CreateModel();

        //Queue Config--------------------------------------------------------------------------------------
        channel.ExchangeDeclare(
            exchange: _rabbitMqConfig.Exchange,
            type: _rabbitMqConfig.ExchangeType
        );

        channel.QueueDeclare(
            _rabbitMqConfig.QueueName,
            exclusive: false);

        channel.QueueBind(
            _rabbitMqConfig.QueueName,
            _rabbitMqConfig.Exchange,
            _rabbitMqConfig.RoutingKey);

        channel.Dispose();
    }

    private static List<RabbitEndpoint> ConvertHostNames(string hostNames)
    {
        var hosts = hostNames.Split(',');
        return hosts.Select(h => new RabbitEndpoint(h)).ToList();
    }
}