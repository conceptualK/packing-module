using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;
using static RestAPI.Controllers.uLoginController;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class uGetDepartmentController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string cmd = "SELECT DEPT_ID,DEPT_NAME FROM SAJET.SYS_DEPT WHERE ENABLED = 'Y' ORDER BY DEPT_NAME ASC ";
                DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                string sJson = string.Empty;
                string msg = string.Empty;
                string status = string.Empty;
                try
                {

                    string result = string.Empty;

                    result = $@"""Message"":[";
                    foreach (DataRow row in dt.Rows)
                    {
                        result += @"{";
                        foreach (DataColumn column in dt.Columns)
                        {
                            result += "\"" + column.ColumnName.ToLower() + "\" : \"" + row[column].ToString() + "\",";
                        }
                        result = result.Substring(0, result.Length - 1);
                        result += @"},";
                    }

                    result = result.Substring(0, result.Length - 1);
                    result += @"],";
                    result = result.Substring(0, result.Length - 1);
                    //  Debug.WriteLine(result);

                    msg = result;
                    status = "00000";
                }
                catch (Exception ex) { status = "00001"; msg = ex.Message; }


                string retrunMSG = "{\r\n\"MsgCode\":\"" + status + "\",\r\n" + msg + "\r\n}";

                return Ok(retrunMSG);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
