using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using Newtonsoft.Json;
using RestAPI.ExternalClass;
using System;
using System.Data;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class fGetOrderController : Controller
    {

        [HttpGet]
        public IActionResult Get()
        {
            string status = string.Empty;
            try
            {
                string cmd = "SELECT ORDER_ID, PO, ISSUE_DATETIME, CATEGORY, PD_ORDER, ITEM_DES, MD_TYPE, MD_GRADE, PD_QTY, QC_QTY, WH_QTY, PD_STATUS, PD_DATETIME, REMARK AS COMMENT1, D_TO, D_DATETIME, PACK_STYLE, INVOICE_NO, STATUS, UPDATETIME, FLAG, WH_DATE, WH_STATUS FROM SAJET.TH_G_MD_PACKING_ORDER WHERE FLAG = 'T' ORDER BY ISSUE_DATETIME DESC";
                DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                //

               string output = ConvertDataTableToJson(dt);


                if (dt.Rows.Count > 0)
                {
                    status = "00000";
                    output = ConvertDataTableToJson(dt);
                }
                else
                {
                    status = "00001";
                    output = "\"no contents\"";
                }

                string retrunMSG = "{\r\n\"MsgCode\":\"" + status + "\",\r\n\"Message\":" + output + "\r\n}";


                //string sJson =  string.Empty;
                //string msg = string.Empty;
                //string status = string.Empty;
                //try
                //{

                //    string result = string.Empty;

                //    result = $@"""Message"":[";
                //    foreach (DataRow row in dt.Rows)
                //    {
                //        result += @"{";
                //        foreach (DataColumn column in dt.Columns)
                //        {
                //            result += "\"" + column.ColumnName.ToLower() + "\" : \"" + row[column].ToString() + "\",";
                //        }
                //        result = result.Substring(0, result.Length - 1);
                //        result += @"},";
                //    }

                //    result = result.Substring(0, result.Length - 1);
                //    result += @"],";
                //    result = result.Substring(0, result.Length - 1);
                //  //  Debug.WriteLine(result);

                //    msg = result;
                //    status = "00000";
                //}
                //catch (Exception ex) { status = "00001"; msg = ex.Message; }


                //string retrunMSG = "{\r\n\"MsgCode\":\"" + status + "\",\r\n"  + msg + "\r\n}";

                return Ok(retrunMSG);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        static string ConvertDataTableToJson(DataTable dt)
        {
            // Create a clone of the DataTable with lowercase column names
            DataTable dtLowerCase = dt.Clone();
            foreach (DataColumn column in dtLowerCase.Columns)
            {
                column.ColumnName = column.ColumnName.ToLower();
            }

            // Import rows from the original DataTable
            foreach (DataRow row in dt.Rows)
            {
                dtLowerCase.ImportRow(row);
            }

            // Serialize to JSON
            return JsonConvert.SerializeObject(dtLowerCase, Formatting.Indented);
        }
    }
}
