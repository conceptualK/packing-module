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
    public class fConfCommandController : Controller
    {
        [HttpPost]
        public async Task<dynamic> PostWithModelWithFromBody([FromBody] TB_Conditions model)
        {
            string status = "000000";
            string msg = "";
            string retrunMSG = "";
            try
            {
                if (model.operation.ToLower() == "insert")
                {

                    if (model.table.ToUpper() == "SAJET.TH_G_MD_ITEM_DESCRIPTION")
                    {
                        string cmd = $"{model.operation} {model.sections}  {model.table} {model.header_fm} VALUES{model.value_fm} {model.condition} {model.sorted} ";
                        DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }
                    else if (model.table.ToUpper() == "SAJET.SYS_VENDOR")
                    {
                        string cmd = $"{model.operation} {model.sections}  {model.table} {model.header_fm} VALUES{model.value_fm} {model.condition} {model.sorted} ";
                        DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }
                    else
                    {
                        string cmd = $"{model.operation} {model.sections}  {model.table} {model.header_fm} VALUES{model.value_fm} {model.condition} {model.sorted} ";
                        DataTable dt = ClientsUnitsMySql.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }

                    msg = "Insert into Database successfully !!!!";
                    retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + " " + msg + " \"\r\n}";

                }
                else if (model.operation.ToLower() == "update")
                {

                    if (model.table.ToUpper() == "SAJET.TH_G_MD_ITEM_DESCRIPTION")
                    {
                        string cmd = $"{model.operation} {model.table} SET {model.sections}  {model.header_fm} {model.value_fm} WHERE {model.condition} {model.sorted} ";
                        DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }
                    else if (model.table.ToUpper() == "SAJET.SYS_VENDOR")
                    {
                        string cmd = $"{model.operation} {model.table} SET {model.sections}  {model.header_fm} {model.value_fm} WHERE {model.condition} {model.sorted} ";
                        DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }
                    else
                    {
                        string cmd = $"{model.operation} {model.table} SET {model.sections} {model.header_fm} {model.value_fm} WHERE {model.condition} {model.sorted} ";
                        DataTable dt = ClientsUnitsMySql.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }

                    msg = "Insert into Database successfully !!!!";
                    retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + " " + msg + " \"\r\n}";

                }
                else if (model.operation.ToLower() == "delete")
                {
                    msg = "Insert into Database successfully !!!!";

                    if (model.table.ToUpper() == "SAJET.TH_G_MD_ITEM_DESCRIPTION")
                    {
                        string cmd = $"{model.operation} FROM {model.table} WHERE {model.condition} ";
                        DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }
                    else if (model.table.ToUpper() == "SAJET.SYS_VENDOR"){


                      //  string cmd = $"{model.operation} FROM {model.table} WHERE {model.condition} ";
                      //  DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                     //   Debug.WriteLine(cmd);

                        msg = "This table cannot use DELETE function recommand use UPDATE !!!!";
                    }
                    else
                    {
                        string cmd = $"{model.operation} FROM {model.table} WHERE {model.condition} ";
                        DataTable dt = ClientsUnitsMySql.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }

                 
                    retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + " " + msg + " \"\r\n}";

                }

                else if (model.operation.ToLower() == "select")
                {
                    DataTable dt;
                    if (model.table.ToUpper() == "SAJET.TH_G_MD_ITEM_DESCRIPTION")
                    {
                        string cmd = $"{model.operation} {model.sections} FROM {model.table} {model.condition} {model.sorted} ";
                        dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }
                    else if (model.table.ToUpper() == "SAJET.SYS_VENDOR")
                    {

                        string cmd = $"{model.operation} {model.sections} FROM {model.table} {model.condition} {model.sorted} ";
                        dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }
                    else
                    {
                        string cmd = $"{model.operation} {model.sections} FROM {model.table} {model.condition} {model.sorted} ";
                        dt = ClientsUnitsMySql.ExecuteWithQuery(cmd);

                        Debug.WriteLine(cmd);
                    }

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

                    retrunMSG = "{\r\n\"MsgCode\":\"" + status + "\",\r\n" + msg + "\r\n}";

                }
                else
                {
                    status = "000001";
                    msg = "operations incorrect formatiing ";
                    retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + " " + msg + " \"\r\n}";
                }

                return Ok(retrunMSG);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class TB_Conditions()
    {
        public string table { get; set; }
        public string operation { get; set; }
        public string sections { get; set; }
        public string condition { get; set; }
        public string sorted { get; set; }
        public string header_fm { get; set; }
        public string value_fm { get; set; }


    }
}
