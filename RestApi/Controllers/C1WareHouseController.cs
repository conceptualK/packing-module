using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestAPI.ExternalClass;
using SkiaSharp;
using System;
using System.Data;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class C1WareHouseController : Controller
    {
        [HttpPost]
        public async Task<dynamic> PostWithFromBody([FromBody] WHinfo model)
        {
            string msg = string.Empty;
            string status = "00001";
            bool bypass_ch = false;
            string cmd = string.Empty;
            int pd_qty = 0;
            int d_qty = 0;
            try
            {
                string pallet_sub = model.pallet_id.Substring(0, 2);
                if (pallet_sub == "PA")
                {
                    cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = '{model.order_id}' AND FLAG = 'T'";
                    DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                    pd_qty = Convert.ToInt32(dt.Rows[0]["PD_QTY"].ToString());
                    if (dt != null)
                    {
                        if (dt.Rows[0]["QC_QTY"].ToString().Length > 0)
                        {
                            d_qty = Convert.ToInt32(dt.Rows[0]["QC_QTY"].ToString());
                        }
                        if (pd_qty == d_qty)
                        {
                            int wh_qty = 0;
                            cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = '{model.order_id}' AND FLAG='T'";
                            dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);



                            try
                            {
                                if (Convert.ToInt32(dt.Rows[0]["WH_QTY"].ToString()) != 0)
                                {

                                    wh_qty = Convert.ToInt32(dt.Rows[0]["WH_QTY"].ToString());

                                }
                            }
                            catch { }
                            if (wh_qty < pd_qty)
                            {
                                bypass_ch = true;
                            }

                        }
                        else { msg = $"\"message\":\"production quality [{pd_qty}]  is not equal delivery quality [{d_qty}]\""; bypass_ch = false; }


                        if (bypass_ch)
                        {
                            cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE PALLET_ID = '{model.pallet_id}' AND RECIPE_ID = '{model.order_id}' AND FLAG = 'QC'";
                            dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                            int just_check = 0;
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string ch_judgement = dt.Rows[i]["JUDGEMENT"].ToString();
                                if (ch_judgement == "OK")
                                {
                                    just_check++;
                                }

                            }

                            int count_dt = 0;
                            if (just_check == 2)
                            {
                                count_dt = dt.Rows.Count;
                            }
                            else { count_dt = 0; }

                            if (count_dt > 0)
                            {
                                try
                                {
                                    cmd = $"SELECT * FROM SAJET.TH_G_MD_MARK_LOCATION WHERE ID = '{model.location}'";
                                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                                    string str_floor = dt.Rows[0]["FLOOR"].ToString();
                                    string room = dt.Rows[0]["ROOM"].ToString();
                                    cmd = $"UPDATE SAJET.TH_G_MD_PACKING_INVENTORY SET PL_LOCATION = '{model.location}-{str_floor}-{room}', FLAG='WH' WHERE PALLET_ID = '{model.pallet_id}' AND RECIPE_ID = '{model.order_id}' AND FLAG ='QC'";
                                    ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                                    msg = $"\"message\":\"Collect WareHouse successfully at location[{model.location}]\"";
                                    status = "00000";
                                }
                                catch (Exception ex) { status = "00001"; msg = ex.Message; }

                                cmd = $"SELECT DISTINCT(PALLET_ID) FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE RECIPE_ID = '{model.order_id}' AND FLAG = 'WH'";
                                dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                                int count_pl = Convert.ToInt32(dt.Rows.Count);

                                cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_ORDER WHERE ORDER_ID = '{model.order_id}' AND FLAG = 'T'";
                                dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                                if (dt.Rows[0]["WH_QTY"] == dt.Rows[0]["PD_QTY"])
                                {

                                    msg = $"\"message\":\"This pallet collect at WareHouse finish : [{model.order_id}].\"";

                                }

                                cmd = $"SELECT PALLET_ID FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE RECIPE_ID = '{model.order_id}' AND FLAG = 'WH'";
                                DataTable md_dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                                count_dt = Convert.ToInt32(md_dt.Rows.Count);

                                cmd = $"UPDATE SAJET.TH_G_MD_PACKING_ORDER SET WH_QTY = '{count_dt}', WH_DATE=SYSDATE, STATUS= 'WH Collect [{count_pl}] pallet [{count_dt}] module' WHERE ORDER_ID = '{model.order_id}' AND FLAG = 'T'";
                                ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);

                            }
                            else
                            {
                                msg = $"\"message\":\"No QC Sampling data : [{model.order_id}], pallet : [{model.pallet_id}] in database.\"";
                                status = "00001";
                            }

                        }
                        else
                        {
                            msg = $"\"message\":\"Check conditions PD and QC QTY. not equal\"";
                        }


                    }
                    else { bypass_ch = false; msg = $"Not found Order_id : [{model.order_id}] in Database"; }
                }
                else
                {

                    cmd = $"SELECT * FROM SAJET.TH_G_MD_MARK_LOCATION WHERE ID = '{model.location}'";
                    DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);


                    if (dt.Rows.Count > 0)
                    {
                        string str_floor = dt.Rows[0]["FLOOR"].ToString();
                        string room = dt.Rows[0]["ROOM"].ToString();

                        cmd = $@"
                        SELECT* FROM(
                            SELECT PALLET_ID FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE PL_LOCATION LIKE '{model.location}%' AND FLAG = 'WH'
                            UNION ALL
                            SELECT PALLET_ID FROM SAJET.TH_G_MD_PACKING_WH WHERE PL_LOCATION LIKE '{model.location}%' AND FLAG = 'WH'
                        ) A
                        GROUP BY PALLET_ID";
                        DataTable dt_i = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        if (dt_i.Rows.Count > 0)
                        {
                            if (model.order_id == "confirm")
                            {
                                cmd = $"UPDATE SAJET.TH_G_MD_PACKING_WH SET PL_LOCATION = '{model.location}-{str_floor}-{room}', FLAG='WH' WHERE PALLET_ID = '{model.pallet_id}' ";
                                ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                                msg = $"\"message\":\"More Collect WareHouse successfully at location[{model.location}]\"";
                                status = "00000";
                            }
                            else
                            {
                                status = "00002";
                                msg = $@"""message"":""found pallet at location more than {dt_i.Rows.Count} pallet!!!"", ""order_id"":""confirm"", ""pallet_id"": ""{model.pallet_id}"", ""location"":""{model.location}"" ";
                            }
                        }
                        else {
                            cmd = $"UPDATE SAJET.TH_G_MD_PACKING_WH SET PL_LOCATION = '{model.location}-{str_floor}-{room}', FLAG='WH' WHERE PALLET_ID = '{model.pallet_id}' ";
                            ClientsUnitsOracle.ExecuteWithNoneQuery(cmd);
                            msg = $"\"message\":\"Collect WareHouse successfully at location[{model.location}]\"";
                            status = "00000";
                        }

                    }
                    else
                    {
                        msg = $"\"message\":\"Cannot find WareHouse at location[{model.location}]\"";
                        status = "00001";
                    }

                }
                string retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":{" + " " + msg + "}\r\n}";
                return Ok(retrunMSG);

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public class WHinfo
        {
            public string location { get; set; }
            public string pallet_id { get; set; }
            public string order_id { get; set; }
        }
    }
}
