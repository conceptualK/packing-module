using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class fReportLocationController : Controller
    {
        string msg = string.Empty;
        string status = string.Empty;
        [HttpPost]
        public async Task<dynamic> PostWithModelWithFromBody([FromBody] report_location model)
        {

            DataTable dt = new DataTable();
            string cmd = string.Empty;

            try
            {

                if (model.action == "get_fl")
                {
                    cmd = $@"SELECT * FROM SAJET.TH_G_MD_MARK_LOCATION WHERE FLOOR = '{model.floor}'";
                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                }
                else if (model.action == "pallet_fl")
                {

                    cmd =
$@"SELECT 
    B.PALLET_ID AS PALLET_ID, 
    B.RECIPE_ID AS ORDER_ID, 
    CASE 
        WHEN B.FLAG = 'W' THEN 'Pack Already'
        WHEN B.FLAG = 'WH' THEN 'Warehouse Collect'
        WHEN B.FLAG = 'D' THEN 'Delete'
        WHEN B.FLAG = 'N' THEN 'NG'
        WHEN B.FLAG = 'OK' THEN 'Delivery Finished'
        WHEN B.FLAG = 'QC' THEN 'QC Sampling'
        WHEN B.FLAG = 'E' THEN 'Replace'     
        ELSE B.FLAG 
    END AS FLAG,
    B.FLAG AS F,
    B.PL_LOCATION
FROM 
    SAJET.TH_G_MD_MARK_LOCATION A 
    LEFT OUTER JOIN SAJET.TH_G_MD_PACKING_INVENTORY B 
    ON A.ID =  SUBSTR(B.PL_LOCATION, 1, INSTR(B.PL_LOCATION, '-') - 1)
WHERE 
    A.ID LIKE '%{model.location_id}%' 
    AND B.FLAG = 'WH'
GROUP BY 
    B.PALLET_ID, 
    B.RECIPE_ID, 
    B.FLAG,
    B.PL_LOCATION
";
                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                    cmd =
$@"SELECT 
    B.PALLET_ID AS PALLET_ID, 
    B.RECIPE_ID AS ORDER_ID, 
    CASE 
        WHEN B.FLAG = 'W' THEN 'Pack Already'
        WHEN B.FLAG = 'WH' THEN 'Warehouse Collect'
        WHEN B.FLAG = 'D' THEN 'Delete'
        WHEN B.FLAG = 'N' THEN 'NG'
        WHEN B.FLAG = 'OK' THEN 'Delivery Finished'
        WHEN B.FLAG = 'QC' THEN 'QC Sampling'
        WHEN B.FLAG = 'E' THEN 'Replace'     
        ELSE B.FLAG 
    END AS FLAG,
    B.FLAG AS F,
    B.PL_LOCATION
FROM 
    SAJET.TH_G_MD_MARK_LOCATION A 
    LEFT OUTER JOIN SAJET.TH_G_MD_PACKING_DELIVERY B 
    ON A.ID =  SUBSTR(B.PL_LOCATION, 1, INSTR(B.PL_LOCATION, '-') - 1)
WHERE 
    A.ID LIKE '%{model.location_id}%' 
    AND B.FLAG = 'WH'
GROUP BY 
    B.PALLET_ID, 
    B.RECIPE_ID, 
    B.FLAG,
    B.PL_LOCATION
";
                    DataTable dt2 = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                    dt.Merge(dt2);



                    cmd = $@"
SELECT 
    B.PALLET_ID AS PALLET_ID, 
    'No Recipe' AS ORDER_ID, 
    CASE 
        WHEN B.FLAG = 'W' THEN 'Pack Already'
        WHEN B.FLAG = 'WH' THEN 'Warehouse Collect'
        WHEN B.FLAG = 'D' THEN 'Delete'
        WHEN B.FLAG = 'N' THEN 'NG'
        WHEN B.FLAG = 'OK' THEN 'Delivery Finished'
        WHEN B.FLAG = 'QC' THEN 'QC Sampling'
        WHEN B.FLAG = 'E' THEN 'Replace'     
        ELSE B.FLAG 
    END AS FLAG,
    B.FLAG AS F,
    B.PL_LOCATION
FROM 
    SAJET.TH_G_MD_MARK_LOCATION A 
    LEFT OUTER JOIN SAJET.TH_G_MD_PACKING_WH B 
    ON A.ID =  SUBSTR(B.PL_LOCATION, 1, INSTR(B.PL_LOCATION, '-') - 1)
WHERE 
    A.ID LIKE '%{model.location_id}%' 
    AND B.FLAG = 'WH'
GROUP BY 
    B.PALLET_ID, 
    B.FLAG,
    B.PL_LOCATION
";
                    DataTable dt3 = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                    dt.Merge(dt3);

                }

                msg = reArrange_dt(dt);
                string retrunMSG = "{\r\n\"MsgCode\":\"" + status + "\",\r\n" + msg + "\r\n}";

                return Ok(retrunMSG);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        
        }
        private string reArrange_dt(DataTable dt)
        {
            string str = String.Empty;

            string sJson = string.Empty;

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

                str = result;
                status = "00000";
            }
            catch (Exception ex) { status = "00001"; msg = ex.Message; }

            return str;
        }
    }

    public class report_location()
    {
        public string action { get; set; }
        public string floor { get; set; }
        public string location_id { get; set; }
        //public string order_id { get; set; }
        //public string pallet_id { get; set; }
        //public string flag { get; set; }
        //public string module_id { get; set; }

    }
}
