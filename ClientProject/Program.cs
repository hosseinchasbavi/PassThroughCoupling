Console.WriteLine("Press any button to send request...");
Console.ReadKey();

var rand = new Random();

var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7174/api/Main/GetData");
var context = $"OfflineContext-{rand.Next()}";
request.Headers.Add("X-Offline-Context", context);

var httpClient = new HttpClient();
var response = await httpClient.SendAsync(request);

Console.WriteLine($"Sent Request:\nContext: {context}");

Console.WriteLine(response.StatusCode);