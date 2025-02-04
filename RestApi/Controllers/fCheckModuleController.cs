using System.Data;
using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class fCheckModuleController : Controller
    {
        [HttpPost]
        public async Task<dynamic> GetWithModuleFromModule([FromBody] CompDatadp model)
        {
            if (model.Data == null || model.Data.Count == 0)
            {
                return "No data provided";
            }

            try
            {
  
                List<string> notFoundModules = new List<string>();

                // Loop through each module serial in the provided data
                foreach (var moduleSerial in model.Data)
                {
                    // SQL query to check if the module exists
                    string cmd = $@"SELECT COUNT(*) FROM sajet.th_g_module WHERE MODULE_SERIAL = '{moduleSerial}'";

                    // Execute the query
                    DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                    string result = dt.Rows[0][0].ToString();

                    // If result is "0", add to the notFoundModules list
                    if (result == "0")
                    {
                        notFoundModules.Add(moduleSerial);
                    }
                }

                // Return the modules not found
                return new { NotFoundModules = notFoundModules };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class CompDatadp
        {
            public List<string>? Data { get; set; }
        }
    }
}
