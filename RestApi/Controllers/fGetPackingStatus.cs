using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestAPI.ExternalClass;
using System;
using System.Data;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class fGetPackingStatusController : Controller
    {

        [HttpPost]
        public async Task<dynamic> PostWithModelWithFromBody([FromBody] PD_status model)
        {
            try
            {
                string cmd =
$@"SELECT 
    RECIPE_ID, 
    PALLET_ID, 
    MODULE_ID,
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
    DETIAL,
    CHARGE_DATE,
    PL_LOCATION,
    TO_CHAR(PACKING_DATE, 'DD-MON-YY') AS PACKDATE , 
    UPDATETIME AS UPDATETIME ,
    (SELECT MD_TYPE FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = A.RECIPE_ID) AS MODULE_TYPE,
    (SELECT D_TO FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = A.RECIPE_ID) AS DELIVERY_TO,
    (SELECT REMARK FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = A.RECIPE_ID) AS REMARK   
FROM 
    SAJET.TH_G_MD_PACKING_INVENTORY A WHERE RECIPE_ID = '{model.order_id}' AND PALLET_ID = '{model.pallet_id}' AND FLAG = '{model.flag}' AND MODULE_ID LIKE '%{model.module_id}%'
ORDER BY 
    UPDATETIME DESC  ";
                DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
if(dt.Rows.Count == 0) { 

                cmd =
$@"SELECT 
    RECIPE_ID, 
    PALLET_ID, 
    MODULE_ID,
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
    DETIAL,
    CHARGE_DATE,
    PL_LOCATION,
    TO_CHAR(PACKING_DATE, 'DD-MON-YY') AS PACKDATE , 
    UPDATETIME AS UPDATETIME ,
    (SELECT MD_TYPE FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = A.RECIPE_ID) AS MODULE_TYPE,
    (SELECT D_TO FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = A.RECIPE_ID) AS DELIVERY_TO,
    (SELECT REMARK FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = A.RECIPE_ID) AS REMARK   
FROM 
    SAJET.TH_G_MD_PACKING_DELIVERY A WHERE RECIPE_ID = '{model.order_id}' AND PALLET_ID = '{model.pallet_id}' AND FLAG = '{model.flag}' AND MODULE_ID LIKE '%{model.module_id}%'
ORDER BY 
    UPDATETIME DESC  ";
                 dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                //dt.Merge(dt2);
                }

                if (dt.Rows.Count == 0)
                {

                    cmd = $@"
SELECT 
    'No Recipe' AS RECIPE,  
    A.PALLET_ID, 
    A.MODULE_ID,
    COUNT(*) AS QTY, 
    CASE 
        WHEN A.FLAG = 'W' THEN 'Pack Already'
        WHEN A.FLAG = 'WH' THEN 'Warehouse Collect'
        WHEN A.FLAG = 'D' THEN 'Delete'
        WHEN A.FLAG = 'N' THEN 'NG'
        WHEN A.FLAG = 'OK' THEN 'Delivery Finished'
        WHEN A.FLAG = 'QC' THEN 'QC Sampling'
        WHEN A.FLAG = 'E' THEN 'Replace'     
        ELSE A.FLAG 
    END AS FLAG,
    A.FLAG AS F,
    D.DETIAL AS DETIAL,
    MAX(A.CHARGE_DATE) AS CHARGE_DATE,
    A.PL_LOCATION,
    TO_CHAR(A.PACKING_DATE, 'DD-MON-YY') AS PACKDATE, 
    A.UPDATETIME AS UPDATETIME,
    'PACK2COLLECT' AS MODULE_TYPE,
    'PACK2COLLECT' AS DELIVERY_TO,
    MAX(A.REMARK) AS REMARK  -- Added MAX for REMARK aggregation
FROM 
    SAJET.TH_G_MD_PACKING_WH A
LEFT JOIN 
    (SELECT 
        MODULE_ID,
        LISTAGG(DETIAL, ', ') WITHIN GROUP (ORDER BY DETIAL) AS DETIAL
     FROM 
        (SELECT DISTINCT MODULE_ID, DETIAL
         FROM SAJET.TH_G_MD_PACKING_WH
         WHERE PALLET_ID = '{model.pallet_id}'  AND FLAG = '{model.flag}' AND MODULE_ID LIKE '%%'
        )
     GROUP BY MODULE_ID
    ) D 
ON A.MODULE_ID = D.MODULE_ID
WHERE 
    A.PALLET_ID = '{model.pallet_id}'  AND A.FLAG = '{model.flag}' AND A.MODULE_ID LIKE '%%'
GROUP BY 
    A.PALLET_ID, 
    A.MODULE_ID,
    A.FLAG,
    D.DETIAL,
    A.PL_LOCATION,
    A.PACKING_DATE,
    A.UPDATETIME
ORDER BY 
    A.UPDATETIME DESC


";
//                    cmd =
//    $@"SELECT 
//        'No Recipe' AS RECIPE,  
//    PALLET_ID, 
//    MODULE_ID,
//   (SELECT COUNT(*) AS QTY FROM SAJET.TH_G_MD_PACKING_WH B WHERE B.PALLET_ID = A.PALLET_ID AND B.FLAG = A.FLAG) AS QTY, 
//    CASE 
//        WHEN FLAG = 'W' THEN 'Pack Already'
//        WHEN FLAG = 'WH' THEN 'Warehouse Collect'
//        WHEN FLAG = 'D' THEN 'Delete'
//        WHEN FLAG = 'N' THEN 'NG'
//        WHEN FLAG = 'OK' THEN 'Delivery Finished'
//        WHEN FLAG = 'QC' THEN 'QC Sampling'
//        WHEN FLAG = 'E' THEN 'Replace'     
//        ELSE FLAG 
//    END AS FLAG ,
//    FLAG AS F,
//    DETIAL,
//    CHARGE_DATE,
//    PL_LOCATION,
//    TO_CHAR(PACKING_DATE, 'DD-MON-YY') AS PACKDATE , 
//    UPDATETIME AS UPDATETIME ,
//    'PACK2COLLECT' AS MODULE_TYPE,
//    'PACK2COLLECT' AS DELIVERY_TO
//FROM 
//    SAJET.TH_G_MD_PACKING_WH A WHERE PALLET_ID = '{model.pallet_id}' AND FLAG = '{model.flag}' AND MODULE_ID LIKE '%{model.module_id}%'
//ORDER BY 
//    UPDATETIME DESC  ";
                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                }

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
    public class PD_status()
    {
        public string order_id { get; set; }
        public string pallet_id { get; set; }
        public string flag { get; set; }
        public string module_id { get; set; }

    }
}
