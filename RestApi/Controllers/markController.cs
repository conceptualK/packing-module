using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;
using System.Diagnostics;

namespace RestAPI.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class markController : Controller
    {

        [HttpPost]
        public async Task<dynamic> PostWithMarkLocationFromBody([FromBody] MLocation Data)
        {
            string msg = string.Empty;
            string status = string.Empty;
            Double? X = Data.X_Bar;
            Double? Y = Data.Y_Bar;
            string? fl = Data.FL;
            string? rm = Data.RM;
            string id = Data.ID;
            try
            {


                if (X != null || Y != null)
                {


                    string cmd1 = $"INSERT INTO SAJET.TH_G_MD_MARK_LOCATION (X,Y,FLOOR,ROOM,ID) VALUES ({X},{Y},'{fl}','{rm}','{id}')";


                    try
                    {
                        ClientsUnitsOracle.ExecuteWithNoneQuery(cmd1);
                        msg = "Insert successfully !!";
                        status = "00000";
                    }
                    catch (Exception ex) { status = "00001"; msg = ex.Message; }



                }
                else
                {

                    msg = "Location is null";
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
        public class MLocation
        {
            public Double X_Bar { get; set; }

            public Double Y_Bar { get; set; }

            public string FL { get; set; }

            public string RM { get; set; }

            public string ID { get; set; }


        }
    }
}
