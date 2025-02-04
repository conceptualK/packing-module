using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;
using System.Diagnostics;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace RestAPI.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class fLocationController : Controller
    {

        [HttpPost]
        public async Task<dynamic> GetWithFromLocation([FromBody] LacationData Data)
        {


            string msg = string.Empty;
            string status = string.Empty;
            string X = string.Empty;
            string Y = string.Empty;
            string fl = string.Empty;
            string Json_p = string.Empty;

            try
            {
                if (Data.PalletId.Substring(0, 2).ToUpper() == "PA")
                {
                    string cmd = $@"SELECT 
                            SUBSTR(
                                A.PL_LOCATION,
                                INSTR(A.PL_LOCATION, '-') + 1,
                                INSTR(A.PL_LOCATION, '-', INSTR(A.PL_LOCATION, '-') + 1) - INSTR(A.PL_LOCATION, '-') - 1
                            ) AS FL,
                            B.X AS X,
                            B.Y AS Y
                        FROM SAJET.TH_G_MD_PACKING_INVENTORY A
                        LEFT JOIN SAJET.TH_G_MD_MARK_LOCATION B 
                            ON SUBSTR(A.PL_LOCATION, 1, INSTR(A.PL_LOCATION, '-') - 1) = B.ID
                        WHERE A.pallet_id = '{Data.PalletId}'
                        GROUP BY A.PL_LOCATION, A.pallet_id,B.X,B.Y";


                    DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);


                    string cmd1 = $@"SELECT PALLET_ID AS PL, MODULE_ID AS MD,
                                        SUBSTR(PL_LOCATION, 1, INSTR(PL_LOCATION, '-') - 1) AS ID,
                                        SUBSTR(PL_LOCATION, INSTR(PL_LOCATION, '-', INSTR(PL_LOCATION, '-') + 1) + 1) AS RM
                                    FROM SAJET.TH_G_MD_PACKING_INVENTORY
                                    WHERE PALLET_ID = '{Data.PalletId}'";

                    DataTable dt1 = ClientsUnitsOracle.ExecuteWithQuery(cmd1);

                    int count = dt1.Rows.Count;

                    if (count > 0)
                    {


                        Json_p += "";

                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {

                            Json_p += "{";

                            for (int j = 0; j < dt1.Columns.Count; j++)
                            {

                                Json_p += "\"" + dt1.Columns[j].ColumnName + "\":\"" + dt1.Rows[i][j].ToString() + "\",";
                            }
                            Json_p = Json_p.Substring(0, Json_p.Length - 1);
                            Json_p += "},";
                        }
                        Json_p = Json_p.Substring(0, Json_p.Length - 1);
                        Json_p += "";


                        Debug.WriteLine(Json_p);
                    }





                    int count_dt = dt.Rows.Count;

                    if (count_dt == 1)
                    {


                        try
                        {
                            var row = dt.Rows[0];
                            //double Fol = Convert.ToDouble(row["FL"]);
                            double x = Convert.ToDouble(row["X"]);
                            double y = Convert.ToDouble(row["Y"]);

                            fl = row["FL"].ToString();
                            X = x.ToString();
                            Y = y.ToString();
                            Debug.WriteLine(fl);
                            Debug.WriteLine(x);
                            Debug.WriteLine(y);
                            msg = "CCall successfully !!";
                            status = "00000";
                        }
                        catch (Exception ex) { status = "00001"; msg = ex.Message; fl = ""; X = ""; Y = ""; Json_p = "[]"; }



                    }
                    else if (count_dt == 0)
                    {

                        status = "00001"; msg = "Not have data"; fl = ""; X = ""; Y = ""; Json_p = "[]";
                    }
                    else
                    {

                        status = "00001"; msg = "Pallet have location data more than one"; fl = ""; X = ""; Y = ""; Json_p = "[]";


                    }
                }

                else if (Data.PalletId.Substring(0, 2).ToUpper() == "PL")
                {

                    string cmd = $@"SELECT 
                            SUBSTR(
                                A.PL_LOCATION,
                                INSTR(A.PL_LOCATION, '-') + 1,
                                INSTR(A.PL_LOCATION, '-', INSTR(A.PL_LOCATION, '-') + 1) - INSTR(A.PL_LOCATION, '-') - 1
                            ) AS FL,
                            B.X AS X,
                            B.Y AS Y
                        FROM SAJET.TH_G_MD_PACKING_WH A
                        LEFT JOIN SAJET.TH_G_MD_MARK_LOCATION B 
                            ON SUBSTR(A.PL_LOCATION, 1, INSTR(A.PL_LOCATION, '-') - 1) = B.ID
                        WHERE A.pallet_id = '{Data.PalletId}'
                        GROUP BY A.PL_LOCATION, A.pallet_id,B.X,B.Y";






                    DataTable dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);


                    string cmd1 = $@"SELECT PALLET_ID AS PL, MODULE_ID AS MD,
                                        SUBSTR(PL_LOCATION, 1, INSTR(PL_LOCATION, '-') - 1) AS ID,
                                        SUBSTR(PL_LOCATION, INSTR(PL_LOCATION, '-', INSTR(PL_LOCATION, '-') + 1) + 1) AS RM
                                    FROM SAJET.TH_G_MD_PACKING_WH
                                    WHERE PALLET_ID = '{Data.PalletId}'";

                    DataTable dt1 = ClientsUnitsOracle.ExecuteWithQuery(cmd1);

                    int count = dt1.Rows.Count;

                    if (count > 0)
                    {


                        Json_p += "";

                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {

                            Json_p += "{";

                            for (int j = 0; j < dt1.Columns.Count; j++)
                            {

                                Json_p += "\"" + dt1.Columns[j].ColumnName + "\":\"" + dt1.Rows[i][j].ToString() + "\",";
                            }
                            Json_p = Json_p.Substring(0, Json_p.Length - 1);
                            Json_p += "},";
                        }
                        Json_p = Json_p.Substring(0, Json_p.Length - 1);
                        Json_p += "";


                        Debug.WriteLine(Json_p);
                    }

                    int count_dt = dt.Rows.Count;

                    if (count_dt == 1)
                    {


                        try
                        {
                            var row = dt.Rows[0];
                            //double Fol = Convert.ToDouble(row["FL"]);
                            double x = Convert.ToDouble(row["X"]);
                            double y = Convert.ToDouble(row["Y"]);

                            fl = row["FL"].ToString();
                            X = x.ToString();
                            Y = y.ToString();
                            Debug.WriteLine(fl);
                            Debug.WriteLine(x);
                            Debug.WriteLine(y);
                            msg = "CCall successfully !!";
                            status = "00000";
                        }
                        catch (Exception ex) { status = "00001"; msg = ex.Message; fl = ""; X = ""; Y = ""; Json_p = "[]"; }



                    }
                    else if (count_dt == 0)
                    {

                        status = "00001"; msg = "Not have data"; fl = ""; X = ""; Y = ""; Json_p = "[]";
                    }
                    else
                    {

                        status = "00001"; msg = "Pallet have location data more than one"; fl = ""; X = ""; Y = ""; Json_p = "[]";


                    }


                }
                string returnMSG = "{\r\n\"MsgResult\":\"" + status + "\",\r\n\"ErrorMessage\":\"" + msg + "\",\r\n\"FL\" :\"" + fl + "\",\r\n\"X\":\"" + X + "\",\r\n\"Y\":\"" + Y + "\"\r\n,\r\n\"detail\":[" + Json_p + "]\r\n}";

                return Ok(returnMSG);

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
        public class LacationData
        {
            public string? PalletId { get; set; }



        }
    }

}
