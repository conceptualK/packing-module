using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class fCellDataController : Controller
    {

        [HttpPost]
        public async Task<dynamic> GetWithCellFromLocation([FromBody] Gdata Data)
        {

            string json = string.Empty;
            string msg = string.Empty;
            string status = string.Empty;


            try
            {
                string cmd = $@"SELECT DISTINCT B.PALLET_ID AS PL,B.MODULE_ID AS MD
                                                FROM SAJET.TH_G_MD_MARK_LOCATION A 
                                                left OUTER join 
                                                (SELECT PALLET_ID, MODULE_ID,PL_LOCATION FROM SAJET.TH_G_MD_PACKING_INVENTORY UNION ALL SELECT PALLET_ID, MODULE_ID,PL_LOCATION FROM SAJET.TH_G_MD_PACKING_WH) B ON A.ID = SUBSTR(B.PL_LOCATION, 1, INSTR(B.PL_LOCATION, '-') - 1)
                                                WHERE A.ID = '{Data.ID}'  AND B.MODULE_ID NOT LIKE 'x%'";


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
                else
                {

                    msg = "Location is Empty!!";
                    status = "00000";
                    json = "[]";
                }

                string retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + msg + "\",\r\n\"MarkLocation\":" + json + "\r\n}";

                return Ok(retrunMSG);

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
        public class Gdata
        {
            public string? ID { get; set; }



        }
    }
}
