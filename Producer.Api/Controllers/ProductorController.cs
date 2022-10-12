using application.Entities;
using application.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Producer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductorController : ControllerBase
    {

        private readonly ILogger<ProductorController> _logger;
        private readonly IProductService _service;

        public ProductorController(ILogger<ProductorController> logger, IProductService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        public int Post([FromBody] Productor resquest)
        {
            return _service.Generate(resquest);
        }
    }
}
