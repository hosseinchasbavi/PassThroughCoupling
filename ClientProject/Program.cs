using System.Text.Json;

Console.WriteLine("Press any button to send request...");
Console.ReadKey();

var rand = new Random();

var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7174/api/Main/GetData");

var context = new Dictionary<string, string>();
context.Add("OfflineContext1", rand.Next().ToString());
context.Add("OfflineContext2", rand.Next().ToString());
context.Add("OfflineContext3", rand.Next().ToString());

var stringContext = JsonSerializer.Serialize(context);

request.Headers.Add("X-Offline-Context", stringContext);

var httpClient = new HttpClient();
var response = await httpClient.SendAsync(request);

Console.WriteLine($"Sent Request:\nContext: {stringContext}");

Console.WriteLine(response.StatusCode);