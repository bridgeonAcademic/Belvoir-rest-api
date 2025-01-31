using Belvoir.Bll.Services.Admin;
using Belvoir.Bll.DTO.Design;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignController : ControllerBase
    {
        private readonly IDesignService _designService;

        public DesignController(IDesignService designService)
        {
            _designService = designService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDesigns([FromQuery] DesignQueryParameters queryParams)
        {
            var result = await _designService.GetDesignsAsync(queryParams);
            return Ok(result);
        }

        [HttpGet("{designId}")]
        public async Task<IActionResult> GetDesignById(Guid designId)
        {
            var response = await _designService.GetDesignByIdAsync(designId);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddDesign([FromForm] AddDesignDTO designDTO)
        {
            var design = new Design
            {
                Name = designDTO.Name,
                Description = designDTO.Description,
                Category = designDTO.Category,
                Price = designDTO.Price,
                Available = designDTO.Available,
                CreatedBy = designDTO.CreatedBy
            };

            var response = await _designService.AddDesignAsync(design, designDTO.ImageFiles);

            return StatusCode(response.StatusCode, response);
        }
    }
}
