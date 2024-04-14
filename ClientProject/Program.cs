Console.WriteLine("Press any button to send request...");
Console.ReadKey();

var rand = new Random();

var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7174/api/Main/GetData");
request.Headers.Add("X-Offline-Context", $"OfflineContext-{rand.Next()}");

var httpClient = new HttpClient();
var response = await httpClient.SendAsync(request);

Console.WriteLine(response.StatusCode);