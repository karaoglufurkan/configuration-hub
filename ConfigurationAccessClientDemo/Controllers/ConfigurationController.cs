using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationHub.Models;
using ConfigurationHub.Providers;
using DemoClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoClient.Controllers
{
    [ApiController]
    [Route("configurations")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationReader _configurationReader;

        public ConfigurationController(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        public IActionResult Query([FromQuery] string key)
        {
            var applicationConfigurations = _configurationReader.GetApplicationConfigurations();

            if (applicationConfigurations == null)
            {
                return Ok(new List<ConfigurationItemModel>());
            }

            if (key == null)
            {
                return Ok(applicationConfigurations);
            }
            
            return Ok(applicationConfigurations.Where(x => x.Key == key).ToList());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateConfigurationItemRequest request)
        {
            await _configurationReader.SetValue(request.Key, new ConfigurationItemModel
            {
                Key = request.Key,
                Value = request.Value,
                Type = request.Type,
                IsActive = true
            });

            return Ok();
        }
        
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromQuery] string key)
        {
            await _configurationReader.DeleteValue(key);

            return NoContent();
        }
    }
}