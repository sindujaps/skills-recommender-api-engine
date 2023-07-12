using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using skills_recommender_api.enums;

namespace skills_recommender_api.Controllers
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
    }

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _learnNlpEngineUrl;
        private readonly IHttpClientWrapper _httpClientWrapper;

        public RecommendationController(IConfiguration configuration, IHttpClientWrapper httpClientWrapper)
        {
            _configuration = configuration;
            _learnNlpEngineUrl = _configuration["LEARN_NLP_ENGINE_URL"];
            _httpClientWrapper = httpClientWrapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RecommendationQuery query)
        {
            try
            {
                int[] sources = query.Sources;

                if (sources == null || sources.Length == 0)
                {
                    return BadRequest(new { error = "Sources array is empty or null" });
                }

                foreach (int source in sources)
                {
                    if (source == (int)Sources.Learn)
                    {
                        var json = JsonSerializer.Serialize(query);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var learnNlpResponse = await _httpClientWrapper.PostAsync($"{_learnNlpEngineUrl}/recommendations", content);

                        learnNlpResponse.EnsureSuccessStatusCode();
                        var learnNlpResponseBody = await learnNlpResponse.Content.ReadAsStringAsync();
                        Console.WriteLine("Learn NLP Engine Response: " + learnNlpResponseBody);
                    }
                    else
                    {
                        // Handle additional AI engines based on other sources
                    }
                }

                return Ok(new { message = "Recommendation request forwarded successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}