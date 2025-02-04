using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestAPI.ExternalClass;
using System.Data;
using static RestAPI.Controllers.fLocationController;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class fVendorController : Controller
    {
        [HttpPost]
        public async Task<dynamic> GetWithFromLocation([FromBody] VendorData model)
        {
            string retrunMSG = string.Empty;
            string status = string.Empty;
            string msg = string.Empty;

            string vendor_id = string.Empty;
            string vendor_code = string.Empty;
            string vendor_name = string.Empty;
            string update_userid = string.Empty;
            string enabled = string.Empty;
            string vendor_tel = string.Empty;
            string result_s = string.Empty;
           string txt = string.Empty;
            try
            {
                string cmd = $"SELECT * FROM SAJET.TH_G_MD_VENDOR WHERE VENDOR_ID LIKE '1%' ORDER  BY VENDOR_ID DESC ";
                DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                int vendorID = 0;
                if (dt.Rows.Count > 0)
                {
                    vendor_id = dt.Rows[0]["VENDOR_ID"].ToString();
                    vendorID = Convert.ToInt32(vendor_id) + 1;

                    txt = dt.Rows[0]["VENDOR_ID"].ToString();
                }
                else
                {
                    vendor_id = "10000000";
                    vendorID = Convert.ToInt32(vendor_id) + 1;
                    txt = "10000000";
                }

                txt = txt.Substring(3);
                int mix_code = Convert.ToInt32(txt) + 1;
                vendor_code = "P-" + mix_code.ToString("D4");

                if (model.actions == "delete")
                {
                    try
                    {
                        cmd = $"SELECT * FROM SAJET.TH_G_MD_VENDOR WHERE  VENDOR_CODE = '{model.vendor_code}' ";
                        dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        if (dt.Rows.Count > 0)
                        {
                            cmd = $"UPDATE SAJET.TH_G_MD_VENDOR SET ENABLED = 'N' WHERE VENDOR_CODE = '{model.vendor_code}'   ";
                            ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                            status = "00000";
                            msg = $"DELETE SUCCESSFULLY!!";
                        }
                        else { msg = "NOT FOUND TO DELETE [ VENDOR_ID ]"; }
                    }
                    catch (Exception ex) { status = "00001"; msg = $"ACTION->['DELETE']{ex.Message}"; }

                    retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + msg + "\"\r\n}";

                }
                else if (model.actions == "update")
                {
                    try
                    {
                        cmd = $"SELECT * FROM SAJET.TH_G_MD_VENDOR WHERE  VENDOR_CODE = '{model.vendor_code}' ";
                        dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        if (dt.Rows.Count > 0)
                        {
                            cmd = $"UPDATE SAJET.TH_G_MD_VENDOR SET VENDOR_NAME = '{model.vendor_name}', VENDOR_TEL = '{model.vendor_tel}' WHERE VENDOR_CODE = '{model.vendor_code}' ";
                            ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                            status = "00000";
                            msg = $"UPDATE SUCCESSFULLY!!";
                        }
                        else { msg = "NOT FOUND TO UPDATE [ VENDOR_ID ]"; }
                    }
                    catch (Exception ex) { status = "00001"; msg = $"ACTION->['UPDATE']{ex.Message}"; }

                    retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + msg + "\"\r\n}";

                }
                else if (model.actions == "insert")
                {

                    try
                    {
                        cmd = $"SELECT * FROM SAJET.TH_G_MD_VENDOR WHERE  VENDOR_ID = '{vendorID}' ";
                        dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        if (dt.Rows.Count == 0)
                        {

                            cmd = $"INSERT INTO SAJET.TH_G_MD_VENDOR(VENDOR_ID, VENDOR_CODE, VENDOR_NAME, UPDATE_USERID, UPDATE_TIME, ENABLED, VENDOR_TEL)" +
                                  $" VALUES('{vendorID}', '{vendor_code}', '{model.vendor_name}', '10000003', SYSDATE, 'Y','{model.vendor_tel}')";
                            ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                            status = "00000";
                            msg = $"INSERT SUCCESSFULLY!!";
                        }
                        else
                        {
                            status = "00001";
                            msg = $"ALREADY INSERT!!";
                        }


                    }
                    catch (Exception ex) { status = "00001"; msg = $"ACTION->['INSERT']{ex.Message}"; }
                    retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + msg + "\"\r\n}";

                }
                else if (model.actions == "select")
                {
                    cmd = $"SELECT * FROM SAJET.TH_G_MD_VENDOR ORDER BY VENDOR_ID DESC ";
                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                    if (dt.Rows.Count > 0)
                    {
                        result_s = QueryConfig2String.dt2Json_fm(dt, "vendor");

                        //      msg = "\"\" , " + result_s ;
                        status = "00000";
                    }
                    else { status = "00001"; }

                    result_s = result_s.Substring(0, result_s.Length - 1);
                    retrunMSG = "{" + result_s + "}";

                }
                else
                {
                    status = "00001"; msg = $"ACTION->['EMPTY'] action is empty!!!";

                }
                return Ok(retrunMSG);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }

    public class VendorData
    {
        public string? actions { get; set; }
        public string? vendor_code { get; set; }
        public string? vendor_name { get; set; }
        public string? vendor_tel { get; set; }

    }
}
