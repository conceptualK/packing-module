using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class fMarkController : Controller
    {

        [HttpPost]
        public async Task<dynamic> PostWithMarkFromBody([FromBody] LData lData)
        {
            string json = string.Empty;
            string msg = string.Empty;
            string status = string.Empty;

    

            try
            {
                if (!string.IsNullOrEmpty(lData.FL))
                {
                    try
                    {

                        string cmd = $"SELECT X, Y, ID, ROOM FROM SAJET.TH_G_MD_MARK_LOCATION WHERE FLOOR = '{lData.FL}'";
                        DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                        int count_dt = dt.Rows.Count;
                        if (count_dt > 0)
                        {
                            string Json_p = string.Empty;

                            Json_p += "[";

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {

                                Json_p += "{";

                                for (int j = 0; j < dt.Columns.Count; j++)
                                {

                                    Json_p += "\"" + dt.Columns[j].ColumnName + "\":\"" + dt.Rows[i][j].ToString() + "\",";
                                }
                                Json_p = Json_p.Substring(0, Json_p.Length - 1);
                                Json_p += "},";
                            }
                            Json_p = Json_p.Substring(0, Json_p.Length - 1);
                            Json_p += "]";


                            json = Json_p;
                            msg = "Location is OK";
                            status = "00000";
                            Debug.WriteLine(json);
                        }
                        else {

                            msg = "Location is Empty!!";
                            status = "00000";
                            json = "[]";
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


                string retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\""+ msg +"\",\r\n\"MarkLocation\":" + json + "\r\n}";

                return Ok(retrunMSG);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class LData
        {
            public string? FL { get; set; }
        }
    }
}
