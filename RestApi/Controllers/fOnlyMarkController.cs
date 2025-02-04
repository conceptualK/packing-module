using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;
using System.Diagnostics;

namespace RestAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class fOnlyMarkController : Controller
    {

        [HttpPost]
        public async Task<dynamic> PostWithOnlyMarkFromBody([FromBody] IDData Data)
        {
       
            string msg = string.Empty;
            string status = string.Empty;
            string X = string.Empty;
            string Y = string.Empty;



            try
            {
                if (!string.IsNullOrEmpty(Data.ID))
                {
                    try
                    {

                        string cmd = $"SELECT X, Y FROM SAJET.TH_G_MD_MARK_LOCATION WHERE ID = '{Data.ID}'";
                        DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                        int count_dt = dt.Rows.Count;

                        if (count_dt == 1)
                        {

                            var row = dt.Rows[0];

                            double x = Convert.ToDouble(row["X"]);
                            double y = Convert.ToDouble(row["Y"]);

                            X = x.ToString();
                            Y = y.ToString();
                            msg = "Location is OK";
                            status = "00000";



                        }
                        else {


                            status = "00001"; msg = "Not have data"; X = ""; Y = "";


                        }
            
     



                    }
                    catch (Exception ex)
                    {

                        status = "00001"; msg = ex.Message;
                    }
                }


                else
                {

                    msg = "Location is null";
                    status = "00001";

                }


                string returnMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + msg + "\",\r\n\"X\":\"" + X + "\",\r\n\"Y\":\"" + Y + "\"\r\n}";

                return Ok(returnMSG);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class IDData
        {
            public string? ID { get; set; }
        }
    }
}
