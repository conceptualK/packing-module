using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cmp;
using RestAPI.ExternalClass;
using System;
using System.Data;
using System.Diagnostics;
using static Mysqlx.Crud.UpdateOperation.Types;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class fCreateOrderController : Controller
    {
        [HttpPost]
        public async Task<dynamic> PostWithModelWithFromBody([FromBody] CompData model)
        {
            string msg = string.Empty;
            string status = string.Empty;
            DataTable dt = new DataTable();
            string cmd = string.Empty;
            try
            {
                string date_condi = DateTime.Now.ToString("dd-MM-yyyy");
                cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_ORDER WHERE FLAG = 'T' AND ISSUE_DATETIME >TO_DATE('{date_condi}', 'DD-MM-YYYY') ORDER BY UPDATETIME DESC";
                dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                int counting = 0;
                if (dt.Rows.Count > 0)
                {
                    string order_no = dt.Rows[0]["ORDER_ID"].ToString();
                    order_no = order_no.Substring(8, 4);
                    counting = Convert.ToInt32(order_no) + 1;
                }
                else { counting = 1; }

                if (model.action == "update")
                {
              
                    cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = '{model.order_id}'";
                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                    if (dt.Rows.Count > 0)
                    {
                        string issue_dtime = model.issue_date.Replace("T", " ");

                        DateTime date = DateTime.ParseExact(issue_dtime, "yyyy-MM-dd HH:mm",
                                           System.Globalization.CultureInfo.InvariantCulture);




                        cmd = $"UPDATE SAJET.TH_G_MD_PACKING_ORDER SET PO='{model.po}', CATEGORY='{model.category}', PD_ORDER='{model.pd_order}', ITEM_DES='{model.item_des}', " +
                            $"MD_TYPE='{model.md_type}', MD_GRADE='{model.md_grade}', PD_QTY='{model.pd_qty}', REMARK='{model.remark}', D_TO='{model.d_to}', PACK_STYLE='{model.pack_style}', " +
                                     $"INVOICE_NO='{model.invoice_no}', ISSUE_DATETIME =TO_DATE('{date.ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS') WHERE ORDER_ID = '{model.order_id}'";
                        ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);

                        msg = "Update successfully !!";
                        status = "00000";
                    }
                    else
                    {
                        msg = $"Not found OrderID : [{model.order_id}]";
                        status = "00001";
                    }
                  
                }
                else if (model.action == "delete")
                {
                    cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = '{model.order_id}'";
                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                    if (dt.Rows.Count > 0)
                    {
                        cmd = $"UPDATE SAJET.TH_G_MD_PACKING_ORDER SET FLAG = 'F' WHERE ORDER_ID = '{model.order_id}' ";
                        ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);

                        cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE RECIPE_ID = '{model.order_id}'";
                        DataTable dt_n = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                        for (int i = 0; i < dt_n.Rows.Count; i++) {
                            cmd = $"UPDATE SAJET.TH_G_MD_PACKING_INVENTORY SET FLAG = 'D', MODULE_ID='x{dt_n.Rows[i]["MODULE_ID"].ToString()}', " +
                                  $"PALLET_ID='x{dt_n.Rows[i]["PALLET_ID"].ToString()}',RECIPE_ID= 'x{model.order_id}'  WHERE MODULE_ID = '{dt_n.Rows[i]["MODULE_ID"].ToString()}' ";
                            ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                        }

                        msg = "Remove successfully !!";
                        status = "00000";
                    }
                    else
                    {
                        msg = $"Not found OrderID : [{model.order_id}]";
                        status = "00001";
                    }
                }
                else if (model.action == "insert")
                {
                    char[] monthChars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L' };
                    int month = Convert.ToInt32(DateTime.Now.ToString("MM"));

                    int date_i = Convert.ToInt32(DateTime.Now.ToString("dd"));

                    string format_date = Convert.ToInt32(DateTime.Now.ToString("yy")) + monthChars[month - 1].ToString() + date_i.ToString("D2");
                    string order_id = $"OR{format_date}-{counting.ToString("D4")}";


                    string issue_dtime = model.issue_date.Replace("T"," ");

                    //DateTime date = DateTime.ParseExact(issue_dtime, "yyyy-MM-dd HH:mm", null);
                    //Console.WriteLine(date);
                    DateTime date = DateTime.ParseExact(issue_dtime, "yyyy-MM-dd HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);

                   

                    cmd = $"INSERT INTO SAJET.TH_G_MD_PACKING_ORDER(ORDER_ID, ISSUE_DATETIME, CATEGORY, PD_ORDER, ITEM_DES, MD_TYPE, " +
                          $"MD_GRADE, PD_QTY,QC_QTY,WH_QTY, PD_STATUS, REMARK, D_TO, PACK_STYLE, INVOICE_NO, STATUS,UPDATETIME, FLAG, PO) " +
                          $"VALUES('{order_id}',TO_DATE('{date.ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS'),'{model.category}','{model.pd_order}','{model.item_des}','{model.md_type}','{model.md_grade}','{model.pd_qty}','0','0'," +
                          $"'Waiting for pack','{model.remark}','{model.d_to}', '{model.pack_style}', '{model.invoice_no}','Create Order', SYSDATE,'T', '{model.po}')";
                    Debug.WriteLine(cmd);
                    try
                    {
                        ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                        msg = "Insert successfully !!";
                        status = "00000";
                    }
                    catch (Exception ex) { status = "00001"; msg = ex.Message; }
                }
                else
                {
                    msg = "Please assigh action !!";
                    status = "00001";


                }

                string retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + " " + msg + " \"\r\n}";

                return Ok(retrunMSG);
            }
            catch (Exception ex)
            {
                return BadRequest(status );
            }
        }
    }

    public class CompData()
    {
        public string action { get; set; }
        public string order_id { get; set; }
        public string po { get; set; }
        public string category { get; set; }
        public string pd_order { get; set; }
        public string item_des { get; set; }
        public string md_type { get; set; }
        public string md_grade { get; set; }
        public string pd_qty { get; set; }
        public string d_to { get; set; }
        public string remark { get; set; }
        public string pack_style { get; set; }
        public string invoice_no { get; set; }
        public string issue_date { get; set; }
    }
}
