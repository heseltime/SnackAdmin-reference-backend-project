using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using SnackAdmin.Dtos;
using Azure;

namespace SnackAdmin.Controllers
{
    public class WebHookController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WebHookController> _logger;

        public WebHookController(ILogger<WebHookController> logger)
        {
            _logger = logger;
            _httpClient= new HttpClient();
        }

        [HttpPost]
        public async Task<IActionResult> SendOrderToRestaurantWebHook(string? webHookUrl, OrderDto dataToSend)
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);

                // Erstellen der HTTP-Anforderung und Hinzufügen von Daten
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                _logger.LogInformation($"WebHook-Request: URL={webHookUrl}, DATA={json.ToString()}");

                // Senden der HTTP-POST-Anforderung an den Webhook
                var response = await _httpClient.PostAsync(webHookUrl, content);
                _logger.LogInformation($"WebHook-Response: URL={webHookUrl}, DATA={response}");

                // check response from webHook
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurred: {ex.Message}");
                return StatusCode(503, $"An Error occurred: {ex.Message}");
            }
        }
    }
}
