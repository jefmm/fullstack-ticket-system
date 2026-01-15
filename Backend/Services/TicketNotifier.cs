using System.Net.Http.Json;
using Microsoft.Extensions.Options;


namespace Backend.Services;

public class TicketNotifier
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly N8nOptions _options;

    public TicketNotifier(
        IHttpClientFactory httpClientFactory,
        IOptions<N8nOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task NotifyAsync(int id, string name, string email, string message)
{
    try
    {
        Console.WriteLine("[n8n] NotifyAsync CALLED");
        Console.WriteLine($"[n8n] Webhook URL: {_options.WebhookUrl}");
        Console.WriteLine($"[n8n] Payload email={email}, id={id}");

        var client = _httpClientFactory.CreateClient();

        var payload = new
        {
            id,
            name,
            email,
            message,
            secret = _options.Secret
        };

        var response = await client.PostAsJsonAsync(
            _options.WebhookUrl,
            payload
        );

        Console.WriteLine($"[n8n] Response: {(int)response.StatusCode} {response.ReasonPhrase}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[n8n] EXCEPTION: {ex}");
    }
}

}
