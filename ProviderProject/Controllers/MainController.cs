using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProviderProject.Db;
using ProviderProject.Entities;

namespace ProviderProject.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class MainController : ControllerBase
{
    public event RequestSavesEventHandler? RequestSaves;
    private readonly MyContext _dbContext;
    private const string UserHeaderName = "X-Offline-Context";

    public MainController(MyContext dbContext, IPushHandler pushHandler)
    {
        _dbContext = dbContext;

        RequestSaves += pushHandler.SendPush;
    }

    [HttpGet]
    public async Task<IActionResult> GetDataAsync()
    {
        var context = Request.Headers[UserHeaderName].FirstOrDefault();
        
        var deserializedDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(context);
        
        await _dbContext.Request.AddAsync(new Request(context));
        
        var result = await _dbContext.SaveChangesAsync();

        if (result == 0)
        {
            return Problem();
        }

        OnRequestSaves();
        return Ok();
    }

    protected virtual void OnRequestSaves()
    {
        RequestSaves?.Invoke(EventArgs.Empty);
    }
}