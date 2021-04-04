using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleRabbit.Publisher;
using System.Threading.Tasks;

namespace ExampleApiApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IPublishService _pubService;

        public TestController(ILogger<TestController> logger, IPublishService pubService)
        {
            _logger = logger;
            _pubService = pubService;
        }

        [HttpPut]
        public async Task<IActionResult> Push([FromBody] TestDto dto)
        {
            await _pubService.PublishAsync(dto);

            return Ok(dto);
        }
    }
}
