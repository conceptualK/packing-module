using System;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text;
using System.Threading;
using Control_ERP;
using Npgsql;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Reflection;



class Program
{
    private static readonly HttpClient client = new HttpClient();


    //static int counter = 0;

    private static readonly IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) // ตั้งค่า Base Path
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // อ่านไฟล์ appsettings.json
    .Build();

    static void Main()
    {
        try
        {


            //while (true)
            //{
                //string deptTime = configuration["AppSettings:DELAY_TIME"];
                //int delayTime = Convert.ToInt32(deptTime);
    

                string query = "SELECT * FROM public.\"G_MO_BASE\" WHERE \"work_order\" IS NOT NULL AND \"part_no\" IS NOT NULL AND \"status_erp\" = 'DONE' AND \"status_email\" = 'WAIT';";
                DataTable dt = DBConnect.Select(query); // Postgres DB



                if (dt.Rows.Count == 0)
                {
                    //counter++;
                        //Console.WriteLine("Not found new work_order.");
                        //LogFile.WriteLog("Not found new work_order.");


                    //if (counter >= 100) 
                    //{
                    //    Console.Clear();
                    //    counter = 0;
                    //}


                }
                else
                {
                    //counter = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        //Console.WriteLine("----------------------START----------------------");
                        LogFile.WriteLog("----------------------START----------------------");

                        string cmd = "SELECT * FROM SAJET.G_WO_BASE WHERE WORK_ORDER = '" + row["WORK_ORDER"] + "'";
                        DataTable dt2 = DBConOracle.Select(cmd); // Orcle DB

                        if (dt2.Rows.Count == 0)
                        {
                            string Get_PartID = "SELECT PART_ID FROM SAJET.SYS_PART WHERE PART_NO = '" + row["PART_NO"] + "'"; // Get PartID from Oracle
                            DataTable dt3 = DBConOracle.Select(Get_PartID);
                            
                            string Get_PartBOM = "SELECT ITEM_PART_ID, PROCESS_ID FROM SAJET.SYS_BOM WHERE ERP_BOM_NO = '" + row["ERP_BOM_NO"] + "'"; // Get ITEM_BOM from Oracle
                            DataTable dt4 = DBConOracle.Select(Get_PartBOM);
                            
                            if (dt3.Rows.Count == 0 || dt4.Rows.Count == 0)
                            {
                                //Console.WriteLine(string.Format("Not have {0} PART_NO or {1} EPR_BOM_NO in Oracle.", row["PART_NO"], row["ERP_BOM_NO"]));
                                LogFile.WriteLog(string.Format("Not have {0} PART_NO or {1} EPR_BOM_NO in Oracle.", row["PART_NO"], row["ERP_BOM_NO"]));
                            }
                            else {

                                try
                                {
                                    string partID = dt3.Rows[0]["PART_ID"].ToString().Trim();

                                    string insertQuery = "INSERT INTO SAJET.G_WO_BASE (WORK_ORDER, TARGET_QTY, INPUT_QTY, OUTPUT_QTY, PART_ID, WO_STATUS, ERP_BOM_NO) VALUES ('" + row["WORK_ORDER"] + "','" + row["TARGET_QTY"] + "' ,'0','0', '" + partID + "','2', '" + row["ERP_BOM_NO"] + "')";
                                    DBConOracle.Execute(insertQuery);

                                    foreach (DataRow row2 in dt4.Rows)
                                    {
                                        string itemPartID = row2["ITEM_PART_ID"].ToString().Trim();
                                        string processID = row2["PROCESS_ID"].ToString().Trim();

                                        string insertPartBOM = "INSERT INTO SAJET.G_WO_BOM (WORK_ORDER, PART_ID, ITEM_PART_ID, PROCESS_ID, UPDATE_USERID) VALUES ('" + row["WORK_ORDER"] + "', '" + partID + "', '" + itemPartID + "', '" + processID + "', '10000001')";
                                        DBConOracle.Execute(insertPartBOM);
                                    }

                                    //Console.WriteLine(string.Format("Record to G_WO_BASE IS {0} OK.", row["WORK_ORDER"]));
                                    LogFile.WriteLog(string.Format("Record to G_WO_BASE IS {0} OK.", row["WORK_ORDER"]));



                                    if (SendEmail(row["WORK_ORDER"].ToString()) == true)
                                    {
                                        string updateQuery = "UPDATE public.\"G_MO_BASE\" SET \"status_email\" = 'DONE' WHERE \"work_order\" = '" + row["WORK_ORDER"] + "'";
                                        DBConnect.Execute(updateQuery);
                                        //Console.WriteLine("UPDATE DONE STATUS EMAIL");
                                        LogFile.WriteLog(string.Format("UPDATE DONE STATUS EMAIL ({0})", row["WORK_ORDER"]));
                                    }
                                    else {

                                        //Console.WriteLine("Send Email fail");
                                        LogFile.WriteLog(string.Format("Send Email fail ({0})", row["WORK_ORDER"]));
                                    }
                                }

                                catch (Exception ex){

                                    //Console.WriteLine($"Error: {ex.Message}");
                                    LogFile.WriteLog($"Error: {ex.Message}");
                                }
                            }

                   
                        }
                        else {


                            //Console.WriteLine(string.Format("Duplicate Work_order IS {0}.", row["WORK_ORDER"]));
                            LogFile.WriteLog(string.Format("Duplicate Work_order IS {0}.", row["WORK_ORDER"]));

                        }

                    }
                    //Console.WriteLine("----------------------END----------------------");
                    LogFile.WriteLog("----------------------END----------------------");
              
                }

            //    Thread.Sleep(delayTime * 1000);


            //}
       

        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Error: {ex.Message}");
            LogFile.WriteLog($"Error: {ex.Message}");   
        }

    }


    public static bool SendEmail(string WO)
    {

        try
        {
            string cmd = "SELECT * FROM public.\"G_MO_BASE\" WHERE \"work_order\" = '" + WO + "' AND \"part_no\" IS NOT NULL AND \"status_erp\" = 'DONE' AND \"status_email\" = 'WAIT';";

            DataTable dt = DBConnect.Select(cmd);


            string cmd1 = "SELECT * FROM SAJET.G_WO_BASE WHERE WORK_ORDER = '" + WO + "'";

            DataTable dt1 = DBConOracle.Select(cmd1);


            if (dt1.Rows.Count > 0 && dt.Rows.Count > 0)

            {
                if (Email_API(WO) == true) {

                    return true;
                }
                else {
                    return false;
                }

            }
            else {
                return false;
            }

        }
        catch
        {
            return false;

        }


    }

    public static bool Email_API( string WO)
    {
        List<string> emailList = new List<string>();


        string UrlApi = "http://10.19.5.107:5001/api/SendEmail";


        try
        {   // var configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory()) // ตั้งค่า Base Path
            //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // อ่านไฟล์ appsettings.json
            //.Build();
            //control_erp/appsettings.json
            string startupPath = AppContext.BaseDirectory;
            LogFile.WriteLog(startupPath);


            string deptId = configuration["AppSettings:DEPT_ID"];
            if (string.IsNullOrEmpty(deptId))
            {
                //Console.WriteLine(" DEPT_ID not found in appsettings.json");
                LogFile.WriteLog(" DEPT_ID not found in appsettings.json");
                return false;
            }
            else
            {
                //Console.WriteLine($"DEPT_ID IS: {deptId}");

                string cmd = $@"SELECT EMAIL FROM SAJET.SYS_USER WHERE DEPT_ID = '{deptId}'";
                DataTable dt = DBConOracle.Select(cmd);

                if (dt != null && dt.Rows.Count > 0)
                {

                    foreach (DataRow row in dt.Rows)
                    {
                        string email = row["EMAIL"].ToString().Trim();
                        emailList.Add(email); // เพิ่มอีเมลลงใน List
                    }

                    //CallEmailApiAsync(emailList, UrlApi, WO);
                    bool result = CallEmailApiAsync(emailList, UrlApi, WO).Result;

                    if (result == true) {

                        return true;

                    }
                    else
                    {
                        return false;
                    }
                   

                    
                
                }
                else
                {
                    return false;
                }
            }



        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Error in Email_API: {ex.Message}");
            LogFile.WriteLog($"Error in Email_API: {ex.Message}");
            return false;
        }
    }


    public static async Task<bool> CallEmailApiAsync(List<string> emailList, string apiUrl, string WO)
    {
   
        try
        {
         
            var payload = new
            {
                recipient = emailList, 
                subject = "TEST API", 
                text = @$"Open {WO} Success"
            };

            // แปลง Object เป็น JSON String
            string jsonString = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // เรียก POST API
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

      
            if (response.IsSuccessStatusCode)
            {
                //Console.WriteLine("Email API called successfully.");
                LogFile.WriteLog(string.Format("Email API called successfully {0}.", WO));

                return true;
            }
            else
            {
                //Console.WriteLine($"API call failed. Status Code: {response.StatusCode}");
                LogFile.WriteLog(string.Format("API call failed {0}. Status Code: {1}", WO, response.StatusCode));

                return false;
            }
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Error in CallEmailApiAsync: {ex.Message}");
            LogFile.WriteLog(string.Format($"Error in CallEmailApiAsync: {ex.Message}"));
            return false;
        }
    }


}



