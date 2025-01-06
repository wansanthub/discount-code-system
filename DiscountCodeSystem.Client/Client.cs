using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using DiscountCodeSystem.Core.Entity;
public class Client
{
        private const string OfflineStorageFile = "offline_operations.json";
    private readonly HubConnection _connection;
    private readonly ConcurrentQueue<string> _offlineQueue;
    public Client(string serverUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(serverUrl + "/discountHub")
            .Build();
        _offlineQueue = LoadOfflineOperations();
    }
    public async Task StartAsync()
    {
        _connection.Closed += async (error) =>
        {
            Console.WriteLine("Disconnected. Retrying in 5 seconds...");
            await Task.Delay(5000);
            await StartAsync();
        };
        await _connection.StartAsync();
        Console.WriteLine("Connected to server.");
        await SynchronizeOfflineOperations();
    }
    public async Task GenerateCodes(ushort count, byte length)
    {
        try
        {
            var response = await _connection.InvokeAsync<GenerateResponse>("GenerateCodes", new GenerateRequest(count, length));
            if (response.Result)
            {
                Console.WriteLine("Codes generated successfully:");
                foreach (var code in response.Codes ?? [])
                {
                    Console.WriteLine(code);
                }
            }
            else
            {
                Console.WriteLine("Failed to generate codes.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }
    public async Task UseCode(string code)
    {
      try
      {
          var response = await _connection.InvokeAsync<UseCodeResponse>("UseCodeAsync", new UseCodeRequest(code));
          Console.WriteLine(response.Result == 0 ? "Code used successfully." : "Failed to use code.");
      }
      catch (Exception)
      {
          Console.WriteLine("Error: Saving for offline use.");
          SaveOfflineOperation($"USE:{code}");
      }
    }
    private void SaveOfflineOperation(string operation)
    {
        _offlineQueue.Enqueue(operation);
        File.WriteAllText(OfflineStorageFile, JsonSerializer.Serialize(_offlineQueue));
    }
    private static ConcurrentQueue<string> LoadOfflineOperations()
    {
        if (!File.Exists(OfflineStorageFile)) return new ConcurrentQueue<string>();
        var content = File.ReadAllText(OfflineStorageFile);
        return JsonSerializer.Deserialize<ConcurrentQueue<string>>(content) ?? new ConcurrentQueue<string>();
    }
    private async Task SynchronizeOfflineOperations()
    {
            while (_offlineQueue.TryDequeue(out var operation))
            {
                var parts = operation.Split(":");

                if (parts[0] == "GENERATE" && parts.Length == 3)
                {
                    await GenerateCodes(ushort.Parse(parts[1]), byte.Parse(parts[2]));
                }
                else if (parts[0] == "USE" && parts.Length == 2)
                {
                    await UseCode(parts[1]);
                }
            }

            File.Delete(OfflineStorageFile);
    }
}