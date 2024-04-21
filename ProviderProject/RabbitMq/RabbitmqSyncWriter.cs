using System.Text;
using RabbitMQ.Client;

namespace ProviderProject.RabbitMq;

public class RabbitmqSyncWriter
{
    private readonly string _exchange;
    private readonly bool _shouldSend;
    private readonly string _routingKey;
    private readonly IConnection _connection;
    private readonly bool _isPublisherConfirm;
    private readonly SemaphoreSlim _semaphoreSlim;
    private const int TimoutForReceivingAckInSeconds = 3;
    private const int MaxChannelCount = 2046;

    public RabbitmqSyncWriter(RabbitMqConfig config)
    {
        _shouldSend = config.ShouldSend;
        _isPublisherConfirm = config.IsPublisherConfirm;

        if (!config.ShouldSend) return;

        _exchange = config.Exchange;
        _routingKey = config.RoutingKey;

        var factory = new ConnectionFactory
        {
            UserName = config.Username,
            Password = config.Password,
            ClientProvidedName = config.QueueName,
            AutomaticRecoveryEnabled = config.AutomaticRecoveryEnabled
        };

        var endpoints = new List<AmqpTcpEndpoint>();

        var temp = config.HostName.Split(':');

        var url = temp[0];
        var port = Convert.ToInt32(temp[1]);

        endpoints.Add(new AmqpTcpEndpoint(url, port));

        _connection = factory.CreateConnection(endpoints);

        _semaphoreSlim = new SemaphoreSlim(MaxChannelCount);
    }

    public void SendByRoutingKey(string message, string headerKey, string headerValue)
    {
        if (_shouldSend)
        {
            using (var channel = _connection.CreateModel())
            {
                _semaphoreSlim.Wait();

                var properties = channel.CreateBasicProperties();

                if (_isPublisherConfirm)
                {
                    channel.ConfirmSelect();
                }

                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                properties.Headers = new Dictionary<string, object>();
                properties.Headers.Add(headerKey, headerValue);

                channel.BasicPublish(_exchange,
                    _routingKey,
                    properties,
                    Encoding.UTF8.GetBytes(message));

                if (_isPublisherConfirm && !channel.WaitForConfirms(TimeSpan.FromSeconds(TimoutForReceivingAckInSeconds)))
                {
                    throw new TimeoutException($"Publisher confirms timed out for the Message: {message}");
                }

                _semaphoreSlim.Release();
            }
        }
    }
}