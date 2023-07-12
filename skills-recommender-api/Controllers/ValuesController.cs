using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace skills_recommender_api.Controllers
{
    [Route("/")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public async Task <IActionResult> Get(){

            return Ok("hello");
        }
    }
}
