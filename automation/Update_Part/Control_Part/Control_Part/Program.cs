using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Control_Part;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program {



    private static readonly HttpClient client = new HttpClient();

    public static async Task Main(string[] args)
    {


        string apiUrl = "http://10.19.5.107:5001/api/MOInfomation/update_part";

        // เรียกใช้ method CallPast
        string result_s = await CallPast(apiUrl);
        var result = JsonConvert.DeserializeObject<JObject>(result_s);

        // เข้าถึงข้อมูล MsgCode
        //Console.WriteLine($"MsgCode: {result["part_info"]}");

        var Msg = result["MsgCode"];

        int count_U = 0;
        
        int count_I = 0;

        

        try
        {
            if (result != null)
            {

                if (Msg.ToString() == "00000")
                {
                    Console.WriteLine($"Start control Part..." + DateTime.Now.ToString("dd/MM/yyyy"));
                    var partInfoArray = result["part_info"];

                    // แปลง JSON Array เป็น DataTable
                    DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(partInfoArray.ToString());

                    foreach (DataRow row in dataTable.Rows)
                    {
                       // Console.WriteLine($"Part No: {row["PART_NO"]}");

                        string cmd = $@"SELECT * FROM SAJET.SYS_PART WHERE PART_NO = '" + row["PART_NO"].ToString().Trim() + "' AND PART_ID IS NOT NULL";

                        DataTable dt = DBOracle.Select(cmd);


                        if (dt.Rows.Count > 0)
                        {
                            string strDate = row["UPDATE_TIME"].ToString();

                            var date = DateTime.Parse(strDate, new CultureInfo("en-US", true));

                            string updatePart = $@"
                                            UPDATE SAJET.SYS_PART
                                            SET PART_ID = '{row["PART_ID"].ToString().Trim()}',
                                                PART_TYPE = '{row["PART_TYPE"] ?? DBNull.Value}',
                                                SPEC1 = '{row["SPEC1"].ToString().Trim()}',
                                                SPEC2 = '{row["SPEC2"] ?? DBNull.Value}',
                                                UPC = '{row["UPC"] ?? DBNull.Value}',
                                                ROUTE_ID = '{row["ROUTE_ID"] ?? DBNull.Value}',
                                                UPDATE_USERID = '{row["UPDATE_USERID"].ToString().Trim()}',
                                                UPDATE_TIME = TO_DATE('{date.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-MM-dd HH24:MI:SS'),
                                                ENABLED = '{row["ENABLED"].ToString().Trim()}',
                                                BURNIN_TIME = '{row["BURNIN_TIME"].ToString().Trim()}',
                                                VERSION = '{row["VERSION"].ToString().Trim()}',
                                                CUST_PART_NO = '{row["CUST_PART_NO"] ?? DBNull.Value}',
                                                VENDOR_PART_NO = '{row["VENDOR_PART_NO"] ?? DBNull.Value}',
                                                MATERIAL_TYPE = '{row["MATERIAL_TYPE"] ?? DBNull.Value}',
                                                EAN = '{row["EAN"] ?? DBNull.Value}',
                                                MODEL_ID = '{row["MODEL_ID"] ?? DBNull.Value}',
                                                LABEL_FILE = '{row["LABEL_FILE"] ?? DBNull.Value}',
                                                RULE_SET = '{row["RULE_SET"] ?? DBNull.Value}',
                                                OPTION1 = '{row["OPTION1"] ?? DBNull.Value}',
                                                OPTION2 = '{row["OPTION2"] ?? DBNull.Value}',
                                                OPTION3 = '{row["OPTION3"] ?? DBNull.Value}',
                                                OPTION4 = '{row["OPTION4"] ?? DBNull.Value}',
                                                OPTION5 = '{row["OPTION5"].ToString().Trim()}',
                                                OPTION6 = '{row["OPTION6"].ToString().Trim()}',
                                                UOM = '{row["UOM"] ?? DBNull.Value}',
                                                MFGER_PART_NO = '{row["MFGER_PART_NO"] ?? DBNull.Value}',
                                                OPTION7 = '{row["OPTION7"] ?? DBNull.Value}',
                                                OPTION8 = '{row["OPTION8"] ?? DBNull.Value}',
                                                OPTION9 = '{row["OPTION9"] ?? DBNull.Value}',
                                                OPTION10 = '{row["OPTION10"] ?? DBNull.Value}',
                                                OPTION11 = '{row["OPTION11"] ?? DBNull.Value}',
                                                OPTION12 = '{row["OPTION12"] ?? DBNull.Value}',
                                                OPTION13 = '{row["OPTION13"] ?? DBNull.Value}',
                                                OPTION14 = '{row["OPTION14"] ?? DBNull.Value}',
                                                OPTION15 = '{row["OPTION15"] ?? DBNull.Value}',
                                                ERP_ID = '{row["ERP_ID"] ?? DBNull.Value}',
                                                R_LAST_UPDATE_DATE = '{row["R_LAST_UPDATE_DATE"] ?? DBNull.Value}',
                                                I_LAST_UPDATE_DATE = '{row["I_LAST_UPDATE_DATE"] ?? DBNull.Value}',
                                                ERP_DETAIL_ID = '{row["ERP_DETAIL_ID"] ?? DBNull.Value}',
                                                LOT_SIZE = '{row["LOT_SIZE"] ?? DBNull.Value}',
                                                MAC_RULE_ID = '{row["MAC_RULE_ID"] ?? DBNull.Value}',
                                                PART_OWNER_ID = '{row["PART_OWNER_ID"] ?? DBNull.Value}',
                                                PART_VENDOR_ID = '{row["PART_VENDOR_ID"] ?? DBNull.Value}',
                                                INVENTORY_UNIT = '{row["INVENTORY_UNIT"] ?? DBNull.Value}',
                                                BOM_UNIT = '{row["BOM_UNIT"] ?? DBNull.Value}',
                                                PURCHASE_UNIT = '{row["PURCHASE_UNIT"] ?? DBNull.Value}',
                                                SELL_UNIT = '{row["SELL_UNIT"] ?? DBNull.Value}',
                                                ERP_BOM_NO = '{row["ERP_BOM_NO"] ?? DBNull.Value}',
                                                ROUTE_NO = '{row["ROUTE_NO"] ?? DBNull.Value}',
                                                LOCATION_ID = '{row["LOCATION_ID"] ?? DBNull.Value}',
                                                MILLING_LIMIT_TIME = '{row["MILLING_LIMIT_TIME"] ?? DBNull.Value}',
                                                BOM_NO = '{row["BOM_NO"] ?? DBNull.Value}'
                                                 WHERE PART_NO = '" + row["PART_NO"].ToString().Trim() + "' AND PART_ID IS NOT NULL";

                            DBOracle.Execute(updatePart);
                            //Console.WriteLine("Update");

                            count_U++;
                        }
                        else
                        {
                            string strDate = row["UPDATE_TIME"].ToString();

                            var date = DateTime.Parse(strDate, new CultureInfo("en-US", true));
                            string GetNewPartID = $@"SELECT MAX(PART_ID) + 1 AS NEWID FROM SAJET.SYS_PART";
                            DataTable NewPartID = DBOracle.Select(GetNewPartID);

                            string NewPartIDInt = NewPartID.Rows[0]["NEWID"].ToString();

                            string NewPartIDString = $@"INSERT INTO SAJET.SYS_PART (PART_ID,PART_NO,PART_TYPE,SPEC1,SPEC2,UPC,ROUTE_ID,UPDATE_USERID,UPDATE_TIME,ENABLED,BURNIN_TIME,VERSION,CUST_PART_NO,VENDOR_PART_NO,MATERIAL_TYPE,EAN,MODEL_ID,LABEL_FILE,
                                                RULE_SET,OPTION1,OPTION2,OPTION3,OPTION4,OPTION5,OPTION6,UOM,MFGER_PART_NO,OPTION7,OPTION8,OPTION9,OPTION10,OPTION11,OPTION12,OPTION13,OPTION14,OPTION15,ERP_ID,R_LAST_UPDATE_DATE,I_LAST_UPDATE_DATE,ERP_DETAIL_ID,
                                                LOT_SIZE,MAC_RULE_ID,PART_OWNER_ID,PART_VENDOR_ID,INVENTORY_UNIT,BOM_UNIT,PURCHASE_UNIT,SELL_UNIT,ERP_BOM_NO,ROUTE_NO,LOCATION_ID,MILLING_LIMIT_TIME,BOM_NO)
                                                VALUE ('{NewPartIDInt}','{row["PART_NO"].ToString().Trim()}','{row["PART_TYPE"] ?? DBNull.Value}','{row["SPEC1"].ToString().Trim()}','{row["SPEC2"] ?? DBNull.Value}','{row["UPC"] ?? DBNull.Value}','{row["ROUTE_ID"] ?? DBNull.Value}',
                                                '{row["UPDATE_USERID"].ToString().Trim()}',TO_DATE('{date.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-MM-dd HH24:MI:SS'),'{row["ENABLED"].ToString().Trim()}','{row["BURNIN_TIME"] ?? DBNull.Value}','{row["VERSION"].ToString().Trim()}',
                                                '{row["CUST_PART_NO"] ?? DBNull.Value}','{row["VENDOR_PART_NO"] ?? DBNull.Value}','{row["MATERIAL_TYPE"] ?? DBNull.Value}','{row["EAN"] ?? DBNull.Value}','{row["MODEL_ID"] ?? DBNull.Value}','{row["LABEL_FILE"] ?? DBNull.Value}',
                                                '{row["RULE_SET"] ?? DBNull.Value}','{row["OPTION1"] ?? DBNull.Value}','{row["OPTION2"] ?? DBNull.Value}','{row["OPTION3"] ?? DBNull.Value}','{row["OPTION4"] ?? DBNull.Value}','{row["OPTION5"] ?? DBNull.Value}','{row["OPTION6"].ToString().Trim()}',
                                                '{row["UOM"] ?? DBNull.Value}','{row["MFGER_PART_NO"] ?? DBNull.Value}','{row["OPTION7"] ?? DBNull.Value}','{row["OPTION8"] ?? DBNull.Value}','{row["OPTION9"] ?? DBNull.Value}','{row["OPTION10"] ?? DBNull.Value}','{row["OPTION11"] ?? DBNull.Value}',
                                                '{row["OPTION12"] ?? DBNull.Value}','{row["OPTION13"] ?? DBNull.Value}','{row["OPTION14"] ?? DBNull.Value}','{row["OPTION15"] ?? DBNull.Value}','{row["ERP_ID"] ?? DBNull.Value}','{row["R_LAST_UPDATE_DATE"] ?? DBNull.Value}','{row["I_LAST_UPDATE_DATE"] ?? DBNull.Value}',
                                                '{row["ERP_DETAIL_ID"] ?? DBNull.Value}','{row["LOT_SIZE"] ?? DBNull.Value}','{row["MAC_RULE_ID"] ?? DBNull.Value}','{row["PART_OWNER_ID"] ?? DBNull.Value}','{row["PART_VENDOR_ID"] ?? DBNull.Value}','{row["INVENTORY_UNIT"] ?? DBNull.Value}','{row["BOM_UNIT"] ?? DBNull.Value}',
                                                '{row["PURCHASE_UNIT"] ?? DBNull.Value}','{row["SELL_UNIT"] ?? DBNull.Value}','{row["ERP_BOM_NO"] ?? DBNull.Value}','{row["ROUTE_NO"] ?? DBNull.Value}','{row["LOCATION_ID"] ?? DBNull.Value}','{row["MILLING_LIMIT_TIME"] ?? DBNull.Value}','{row["BOM_NO"] ?? DBNull.Value}')";

                            DBOracle.Execute(NewPartIDString);

                            //Console.WriteLine("New Insert");

                            count_I++;
                        }
                    }

                    Console.WriteLine(String.Format("Part Count Update: {0}", count_U));
                    Console.WriteLine(String.Format("Part Count Insert: {0}", count_I));

                }

            }
            else
            {
                Console.WriteLine("Cannot get data.");
            }
        }
        catch (Exception ex) {


            Console.WriteLine(ex.Message);


        }
    }



    public static async Task<string> CallPast(string apiUrl)
    {


        try
        {
            // เรียก GET API
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            // ตรวจสอบสถานะการตอบกลับ
            if (response.IsSuccessStatusCode)
            {

                string responseData = await response.Content.ReadAsStringAsync();


                return responseData;
            }
            else
            {
                Console.WriteLine($"API call failed. Status Code: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }



}