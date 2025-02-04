using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class fReportGenerateController : Controller
    {

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string cmd =
@"SELECT 
    RECIPE_ID, 
    PALLET_ID, 
   (SELECT COUNT(*) AS QTY FROM SAJET.TH_G_MD_PACKING_INVENTORY B WHERE B.PALLET_ID = A.PALLET_ID AND B.FLAG = A.FLAG) AS QTY, 
    CASE 
        WHEN FLAG = 'W' THEN 'Pack Already'
        WHEN FLAG = 'WH' THEN 'Warehouse Collect'
        WHEN FLAG = 'D' THEN 'Delete'
        WHEN FLAG = 'N' THEN 'NG'
        WHEN FLAG = 'OK' THEN 'Delivery Finished'
        WHEN FLAG = 'QC' THEN 'QC Sampling'
        WHEN FLAG = 'E' THEN 'Replace'     
        ELSE FLAG 
    END AS FLAG ,
    FLAG AS F,
    TO_CHAR(PACKING_DATE, 'DD-MON-YY') AS PACKDATE , 
    MAX(UPDATETIME) AS UPDATETIME ,
    (SELECT MD_TYPE FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = A.RECIPE_ID) AS MODULE_TYPE,
    (SELECT D_TO FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = A.RECIPE_ID) AS DELIVERY_TO

FROM 
    SAJET.TH_G_MD_PACKING_INVENTORY A
GROUP BY 
    RECIPE_ID, 
    PALLET_ID, 
    PACKING_DATE,
    FLAG
ORDER BY 
    UPDATETIME DESC ";
                DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);


                cmd =
@"SELECT 
    RECIPE_ID, 
    PALLET_ID, 
   (SELECT COUNT(*) AS QTY FROM SAJET.TH_G_MD_PACKING_DELIVERY B WHERE B.PALLET_ID = A.PALLET_ID AND B.FLAG = A.FLAG) AS QTY, 
    CASE 
        WHEN FLAG = 'W' THEN 'Pack Already'
        WHEN FLAG = 'WH' THEN 'Warehouse Collect'
        WHEN FLAG = 'D' THEN 'Delete'
        WHEN FLAG = 'N' THEN 'NG'
        WHEN FLAG = 'OK' THEN 'Delivery Finished'
        WHEN FLAG = 'QC' THEN 'QC Sampling'
        WHEN FLAG = 'E' THEN 'Replace'     
        ELSE FLAG 
    END AS FLAG ,
    FLAG AS F,
    TO_CHAR(PACKING_DATE, 'DD-MON-YY') AS PACKDATE , 
    MAX(UPDATETIME) AS UPDATETIME ,
    (SELECT MD_TYPE FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = A.RECIPE_ID) AS MODULE_TYPE,
    (SELECT D_TO FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = A.RECIPE_ID) AS DELIVERY_TO

FROM 
    SAJET.TH_G_MD_PACKING_DELIVERY A
GROUP BY 
    RECIPE_ID, 
    PALLET_ID, 
    PACKING_DATE,
    FLAG
ORDER BY 
    UPDATETIME DESC ";
                DataTable dt2 = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                dt.Merge(dt2);



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
