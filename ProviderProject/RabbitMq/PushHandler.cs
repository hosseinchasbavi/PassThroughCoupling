using ProviderProject.Db;

namespace ProviderProject.RabbitMq;

public class PushHandler : IPushHandler
{
    private readonly MyContext _context;
    private readonly RabbitmqSyncWriter _rabbitWriter;
    private const string UserHeaderName = "X-Offline-Context";


    public PushHandler(MyContext context, IConfiguration configuration)
    {
        _context = context;
        var rabbitMqConfig = new RabbitMqConfig();
        configuration.GetSection("RabbitMqConfig").Bind(rabbitMqConfig);

        var topologyMaker = new TopologyMaker(rabbitMqConfig);
        topologyMaker.Make();

        _rabbitWriter = new RabbitmqSyncWriter(rabbitMqConfig);
    }

    public void SendPush(EventArgs args)
    {
        var request = _context.Request.OrderByDescending(r => r.Id).First();

        if (request == null) throw new NullReferenceException($"No Request Found");

        _rabbitWriter.SendByRoutingKey(request.ToString(), UserHeaderName, request.Context);
    }
}