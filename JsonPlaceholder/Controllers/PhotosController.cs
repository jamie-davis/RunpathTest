using System;
using System.Threading.Tasks;
using JsonPlaceholder.PlaceholderOperations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JsonPlaceholder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly PlaceholderDatasetBuilder _builder;
        private readonly ILogger<PhotosController> _logger;

        public PhotosController(PlaceholderDatasetBuilder builder, ILogger<PhotosController> logger)
        {
            _builder = builder;
            _logger = logger;
        }

        // GET: api/Photos
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                return new JsonResult(await _builder.BuildAsync());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to return photo data from api/Photos");
                return NotFound();
            }
        }

        // GET: api/Photos/5
        [HttpGet("{userId:long}")]
        public async Task<ActionResult> Get(long userId)
        {
            try
            {
                return new JsonResult(await _builder.BuildAsync(userId));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to return photo data from api/Photos/{userId}");
                return NotFound();
            }
        }
    }
}
