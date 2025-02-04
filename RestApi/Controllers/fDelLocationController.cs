using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class fDelLocationController : Controller
    {



        [HttpPost]
        public async Task<dynamic> PostWithDelLocationFromBody([FromBody] DelLocation Data)
        {
            string msg = string.Empty;
            string status = string.Empty;
            string Dmain = Data.ID;

            try
            {


                if (Dmain != null)
                {


                    string cmd = $"DELETE FROM SAJET.TH_G_MD_MARK_LOCATION WHERE ID = '{Dmain}'";


                    try
                    {
                        ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                        msg = "Delete successfully !!";
                        status = "00000";
                    }
                    catch (Exception ex) { status = "00001"; msg = ex.Message; }



                }
                else
                {

                    msg = "ID is null";
                    status = "00001";

                }

                string retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + " " + msg + " \"\r\n}";

                return Ok(retrunMSG);

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
        public class DelLocation
        {

            public string ID { get; set; }


        }
    }
}
