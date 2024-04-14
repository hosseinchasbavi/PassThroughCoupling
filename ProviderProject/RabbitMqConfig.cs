namespace ProviderProject;

public class RabbitMqConfig
{
    public string HostName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Exchange { get; set; }
    public string ExchangeType { get; set; }
    public string RoutingKey { get; set; }
    public bool ShouldSend { get; set; }
    public string QueueName { get; set; }
    public bool AutomaticRecoveryEnabled { get; set; }
    public bool IsPublisherConfirm { get; set; }
    public bool IsDurableExchange { get; set; }
    public bool IsDurableQueue { get; set; }
    public bool IsAutoDeleteExchange { get; set; }
    public bool IsAutoDeleteQueue { get; set; }
}