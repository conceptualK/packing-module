using Microsoft.AspNetCore.Mvc;
using Mysqlx.Notice;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using RestAPI.ExternalClass;
using System;
using System.Data;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class C3DeliveryController : Controller
    {
        [HttpPost]
        public async Task<dynamic> PostWithDeliveryFromBody([FromBody] CompDeliData Data)
        {
            string msg = string.Empty;
            string status = string.Empty;
            try
            {
                string cmd = $"SELECT * FROM SAJET.TH_G_MD_PACKING_ORDER WHERE  ORDER_ID = '{Data.order_id}'";
                DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                int count_dt = (dt.Rows.Count > 0) ? dt.Rows.Count:0;

                if (count_dt == 1)
                {

                    string cmd1 = $"SELECT PD_QTY, QC_QTY, WH_QTY, FLAG FROM SAJET.TH_G_MD_PACKING_ORDER WHERE  ORDER_ID = '{Data.order_id}'";
                    DataTable dt1 = ClientsUnitsOracle.ExecuteWithQuery(cmd1);

                    try
                    {
                        string PQTY = dt1.Rows[0]["PD_QTY"].ToString();
                        string QQTY = dt1.Rows[0]["QC_QTY"].ToString();
                        string WHQTY = dt1.Rows[0]["WH_QTY"].ToString();
                        string Flag = dt1.Rows[0]["FLAG"].ToString();


                        if (PQTY == QQTY && QQTY == WHQTY && PQTY == WHQTY && Flag == "T")
                        {
                            //string cmd2 = $"SELECT COUNT(*) AS Count  FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE  RECIPE_ID = '{Data.order_id}' AND FLAG = 'WH'";

                            string cmd2 = $@"
SELECT * FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE  RECIPE_ID = '{Data.order_id}' AND FLAG = 'WH'
UNION ALL 
SELECT *  FROM SAJET.TH_G_MD_PACKING_DELIVERY WHERE  RECIPE_ID = '{Data.order_id}' AND FLAG = 'OK'";
                            DataTable dt2 = ClientsUnitsOracle.ExecuteWithQuery(cmd2);

                            string PackQTY = dt2.Rows.Count.ToString();

                            if (PackQTY == PQTY)
                            {
                                string cmd3 = $"SELECT COUNT(*) AS DCount  FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE  RECIPE_ID = '{Data.order_id}' AND FLAG NOT IN ('WH','N','D')";
                                DataTable dt3 = ClientsUnitsOracle.ExecuteWithQuery(cmd3);

                                string InQTY = dt3.Rows[0]["DCount"].ToString();

                                if (InQTY == "0")
                                {
                                    string cmd4 = $"UPDATE SAJET.TH_G_MD_PACKING_INVENTORY SET FLAG = 'OK' WHERE RECIPE_ID = '{Data.order_id}' AND PALLET_ID = '{Data.pallet_id}' AND FLAG = 'WH'";
                                    ClientsUnitsOracle.ExecuteWithNoneQuery(cmd4);

                                    string cmd5 = $"INSERT INTO SAJET.TH_G_MD_PACKING_DELIVERY (RECIPE_ID ,PACKING_DATE, PALLET_ID, DETIAL, FLAG, UPDATETIME, CHARGE_DATE, MODULE_ID, PL_LOCATION, JUDGEMENT) SELECT RECIPE_ID ,PACKING_DATE, PALLET_ID, DETIAL, FLAG, UPDATETIME, CHARGE_DATE, MODULE_ID, PL_LOCATION, JUDGEMENT FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE RECIPE_ID = '{Data.order_id}'  AND PALLET_ID = '{Data.pallet_id}'";
                                    ClientsUnitsOracle.ExecuteWithNoneQuery(cmd5);

                                    string cmd6 = $"DELETE SAJET.TH_G_MD_PACKING_INVENTORY WHERE RECIPE_ID = '{Data.order_id}'  AND PALLET_ID = '{Data.pallet_id}'";
                                    ClientsUnitsOracle.ExecuteWithNoneQuery(cmd6);


                                    string cmd8 = $"SELECT * FROM SAJET.TH_G_MD_PACKING_INVENTORY WHERE RECIPE_ID = '{Data.order_id}'";
                                    DataTable dt9 = ClientsUnitsOracle.ExecuteWithQuery(cmd8);
                                    if (dt9.Rows.Count == 0)
                                    {

                                        string cmd7 = $"UPDATE SAJET.TH_G_MD_PACKING_ORDER SET FLAG = 'OK' WHERE ORDER_ID = '{Data.order_id}' AND FLAG = 'T'";
                                        ClientsUnitsOracle.ExecuteWithNoneQuery(cmd7);

                                    }

                                    msg = "Update successfully !!";
                                    status = "00000";
                                }
                                else
                                {
                                    msg = "Flag is not WH,N,D !!";
                                    status = "00001";
                                }
                            }
                            else
                            {
                                msg = "QTY order not match packing !!";
                                status = "00001";
                            }
                        }
                        else
                        {
                            msg = "QTY not ready !!";
                            status = "00001";
                        }

                    }
                    catch (Exception ex) { status = "00001"; msg = ex.Message; }
                }
                else
                {
                    msg = "ORDER_ID more than 1 !!";
                    status = "00001";
                }
                string retrunMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + " " + msg + " \"\r\n}";
                return Ok(retrunMSG);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

    public class CompDeliData
    {
        //public string location { get; set; }
        public string pallet_id { get; set; }
        public string order_id { get; set; }

    }
}
