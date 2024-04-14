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

    public MainController(MyContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetDataAsync()
    {
        var context = Request.Headers[UserHeaderName].FirstOrDefault();
        await _dbContext.Request.AddAsync(new Request(context));
        var result = _dbContext.SaveChanges();
        if (result != 0)
        {
            return Ok();
        }

        return Problem();
    }
}