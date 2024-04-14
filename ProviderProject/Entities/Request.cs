namespace ProviderProject.Entities;

public class Request
{
    public Request(string context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public int Id { get; set; }
    public string Context { get; set; }
}