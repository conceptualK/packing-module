using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using Newtonsoft.Json;
using RestApi.Controllers;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetConfDataController : ControllerBase
    {
        //  [HttpPost]
        //  public async Task<dynamic> PostWithModelWithFromBody([FromBody] CompData model)
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string txtResponse = QueryConfig2String.main();
       
                return Ok(txtResponse);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid JSON format: " + ex.Message);
            }
        }
    }

}
