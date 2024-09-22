using Algobot.Worker.Domain.ValueObject;
using Algobot.Worker.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Algobot.Worker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly IPostiveSentimentDataProvider _postiveSentimentData;

        public MainController(IPostiveSentimentDataProvider postiveSentimentData)
        {
            _postiveSentimentData = postiveSentimentData;
        }

        [HttpGet]
        public async Task<IActionResult> Start()
        {
            await _postiveSentimentData.GetSymbols(Interval.FourHour, 3m);

            return Ok();
        }
    }
}
