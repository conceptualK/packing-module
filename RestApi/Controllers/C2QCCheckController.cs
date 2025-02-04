using Aspose.Cells.Drawing;
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
    public class C2QCCheckController : Controller
    {

        [HttpPost]
        public async Task<dynamic> PostWithModelWithFromBody([FromBody] QCcheck model)
        {
            string sJson = string.Empty;
            string result = string.Empty;
            string msg = string.Empty;
            string status = string.Empty;
            string check_lk = string.Empty;
            string cmd = string.Empty;
            bool bypass_al = true;
            bool bypass_all = true;

            try
            {


                string cmd_status = $"SELECT * FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE RECIPE_ID= '{model.order_id}' AND  PALLET_ID  ='{model.pallet_id}' AND MODULE_ID LIKE '%{model.module_id}%' ORDER BY UPDATETIME DESC ";
                DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd_status);

                if (dt.Rows.Count > 0)
                {
                    cmd = $"UPDATE SAJET.TH_G_MD_PACKING_INVENTORY SET FLAG = 'QC',JUDGEMENT = '{model.judge}', DEFECT_CODE='{model.defect_code}' ,MODULE_ID = '{model.module_id}' WHERE RECIPE_ID= '{model.order_id}' AND  PALLET_ID  ='{model.pallet_id}' AND MODULE_ID LIKE '%{model.module_id}%' ";
                    ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                    bypass_al = true;
                }
                else
                {
                    bypass_al = false;
                    msg = "\"ErrorMsg\":\"[]\"";
                    check_lk = "Not found FLAG ['W'] in TH_G_MD_PACKING_INVENTORY";
                    status = "00002";
                }

                if (bypass_al)
                {
                    if (model.judge == "NG")
                    {
                        cmd = $"UPDATE SAJET.TH_G_MD_PACKING_INVENTORY SET FLAG = 'N' , MODULE_ID = 'ng:[{model.module_id}]' ,DEFECT_CODE = '{model.defect_code}' WHERE RECIPE_ID= '{model.order_id}' AND  PALLET_ID  ='{model.pallet_id}' AND MODULE_ID = '{model.module_id}' ";
                        ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                    }
                }


                string cmd_or = $"SELECT * FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = '{model.order_id}' AND FLAG='T' ";
                if (bypass_al)
                {
                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd_or);
                    if (dt.Rows.Count > 0)
                    {
                        int d_qty = 0;
                        int pd_qty = Convert.ToInt32(dt.Rows[0]["PD_QTY"].ToString());
                        if (dt.Rows[0]["QC_QTY"].ToString().Length > 0)
                        {
                            d_qty = Convert.ToInt32(dt.Rows[0]["QC_QTY"].ToString());
                        }

                        if (pd_qty <= d_qty)
                        {
                            bypass_al = false;
                            msg = "\"ErrorMsg\":\"[]\"";
                            check_lk = "Production packing finished ..";
                            status = "00001";
                        }
                    }
                    else
                    {
                        bypass_al = false;
                        msg = "\"ErrorMsg\":\"[]\"";
                        check_lk = "Not found QC in [TH_G_MD_PACKING_ORDER]";
                        status = "00001";
                    }
                }


                if (bypass_al)
                {

                    try
                    {
                        cmd_status = $"SELECT * FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE RECIPE_ID= '{model.order_id}' AND  PALLET_ID  ='{model.pallet_id}' AND MODULE_ID LIKE '%{model.module_id}%' ORDER BY UPDATETIME DESC ";
                        dt = ClientsUnitsOracle.ExecuteWithQuery(cmd_status);

                        if (dt.Rows.Count > 0)
                        {

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



                            cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE  RECIPE_ID= '{model.order_id}' AND  PALLET_ID  ='{model.pallet_id}' AND JUDGEMENT = 'OK' AND FLAG='QC'";
                            dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                            if (dt.Rows.Count == 2)
                            {
                                cmd = $"UPDATE SAJET.TH_G_MD_PACKING_INVENTORY SET FLAG='QC' WHERE RECIPE_ID= '{model.order_id}' AND  PALLET_ID  ='{model.pallet_id}' AND FLAG = 'W'";
                                ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                            }

                            msg = result;
                            status = "00000";
                        }
                        else
                        {
                            msg = $"\"ErrorMsg\":\"Not found data order : [{model.order_id}], pallet : [{model.pallet_id}], module : [{model.module_id}] in database.\"";
                            status = "00001";
                        }



                    }
                    catch (Exception ex) { status = "00001"; msg = ex.Message; }
                }




                cmd_or = $"SELECT * FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = '{model.order_id}' AND FLAG ='T' ";
                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd_or);
                if (dt.Rows.Count > 0)
                {
                    int pd_qty = Convert.ToInt32(dt.Rows[0]["PD_QTY"].ToString());
                    int d_qty = 0;
                    try
                    {
                         d_qty = Convert.ToInt32(dt.Rows[0]["QC_QTY"].ToString());
                    }
                    catch { d_qty = 0; }
                    if (pd_qty == d_qty)
                    {
                        check_lk = "Ready to WareHouse collection";
                        status = "00000";
                        cmd = $"UPDATE SAJET.TH_G_MD_PACKING_ORDER SET PD_STATUS='Finished [Charge, Sampling and Pack2Pallet]',PD_DATETIME=SYSDATE, STATUS='Waiting WareHouse Collect', UPDATETIME=SYSDATE WHERE ORDER_ID = '{model.order_id}' AND FLAG  ='T'";
                        ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                        bypass_all = true;

                    }
                    else { check_lk = $"production quality [{pd_qty}]  is not equal delivery quality [{d_qty}]"; bypass_all = false; }
                }

                if (!bypass_all)
                {
                    cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE  RECIPE_ID= '{model.order_id}'AND PALLET_ID='{model.pallet_id}' AND JUDGEMENT = 'OK' AND FLAG='QC'  ";
                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                    if (dt.Rows.Count == 2)
                    {
                        check_lk = $"You sampling in pallet [{model.pallet_id}] maximum accept [{dt.Rows.Count}]";
                    }
                    else
                    {
                        check_lk = $"Continue packing [{model.pallet_id}] maximum accept [{dt.Rows.Count}]";
                    }
                }

                cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE  RECIPE_ID= '{model.order_id}' AND FLAG='QC'  ";
                dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                cmd = $"UPDATE SAJET.TH_G_MD_PACKING_ORDER SET QC_QTY='{dt.Rows.Count}' WHERE ORDER_ID = '{model.order_id}' AND FLAG  ='T'";
                ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);

                string retrunMSG = "{\r\n\"MsgCode\":\"" + status + "\",\r\n\"Status\":\"" + check_lk + "\",\r\n" + msg + "\r\n}";

                return Ok(retrunMSG);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class QCcheck()
    {
        public string order_id { get; set; }
        public string pallet_id { get; set; }
        public string module_id { get; set; }
        public string judge { get; set; }
        public string defect_code { get; set; }

    }
}
