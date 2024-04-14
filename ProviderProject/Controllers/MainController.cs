using Microsoft.AspNetCore.Mvc;
using ProviderProject.Db;
using ProviderProject.Entities;

namespace ProviderProject.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class MainController : ControllerBase
{
    private readonly MyContext _dbContext;
    private const string UserHeaderName = "X-Offline-Context";
    private readonly RabbitMqConfig _rabbitMqConfig;
    private readonly RabbitmqSyncWriter _rabbitWriter;

    public MainController(MyContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;

        _rabbitMqConfig = new RabbitMqConfig();
        configuration.GetSection("RabbitMqConfig").Bind(_rabbitMqConfig);

        var topologyMaker = new TopologyMaker(_rabbitMqConfig);
        topologyMaker.Make();

        _rabbitWriter = new RabbitmqSyncWriter(_rabbitMqConfig);
    }

    [HttpGet]
    public async Task<IActionResult> GetDataAsync()
    {
        var context = Request.Headers[UserHeaderName].FirstOrDefault();
        await _dbContext.Request.AddAsync(new Request(context));
        var result = _dbContext.SaveChanges();

        if (result == 0)
        {
            return Problem();
        }

        _rabbitWriter.SendByRoutingKey(context, UserHeaderName, context);

        return Ok();
    }
}