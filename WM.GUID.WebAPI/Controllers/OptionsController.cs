#pragma warning disable 1591
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WM.GUID.WebAPI.Controllers
{
    [Route("[controller]")]
    public class OptionsController : ControllerBase
    {
        private readonly IOptions<Config> _options;

        public OptionsController(IOptions<Config> options)
        {
            _options = options;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok($"Database connection string: {_options.Value.Database}");
        }
    }
}