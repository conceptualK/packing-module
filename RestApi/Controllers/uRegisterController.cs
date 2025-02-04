using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestAPI.ExternalClass;
using SkiaSharp;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class uRegisterController : Controller
    {
        [HttpPost]
        public async Task<dynamic> PostWithModelWithFromBody([FromBody] member_info model)
        {
            try
            {
                DataTable dt = new DataTable();
                string cmd = string.Empty;
                string msgcode = "00000";
                string message = string.Empty;
                string txtResponse = string.Empty;

                string password = model.password;
                string hashedPassword = PasswordHasher.HashPassword(password);

                try
                {

                    cmd = $"SELECT * FROM config_member WHERE username = '{model.username}'";
                    dt = ClientsUnitsMySql.ExecuteWithQuery(cmd);


                    bool isMatch = PasswordHasher.VerifyPassword(password, hashedPassword);
                    Debug.WriteLine("Password Match: " + isMatch);




                    if (dt.Rows.Count > 0)
                    {
                        msgcode = "00001";
                        message = "NG, This username had been use.";
                    }
                    else
                    {

                        cmd = $"SELECT DEPT_ID, DEPT_NAME FROM SAJET.SYS_DEPT WHERE DEPT_NAME = '{model.department}' AND ENABLED = 'Y' ORDER BY DEPT_NAME ASC ";
                        dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);

                        if (dt.Rows.Count > 0)
                        {
                            cmd = $"INSERT INTO config_member(username, password, department, q1, fullname,a1) VALUES('{model.username}','{hashedPassword}','{dt.Rows[0]["DEPT_ID"].ToString()}','{model.q1}', '{model.fullname}','{model.a1}')";
                            ClientsUnitsMySql.ExecuteWithNoneQuery(cmd);
                            message = "OK, Register successfully";
                        }
                        else {
                            msgcode = "00001";
                            message = "NG, Not found department name ...";
                        }
                    }

                }
                catch
                {
                    msgcode = "00001";
                    message = "NG, Error in Database";
                }

                txtResponse = "{\r\n\"MsgResult\":\"" + msgcode + "\",\r\n\"ErrorMessage\":\"" + " " + message + " \"\r\n}";


                return Ok(txtResponse);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid JSON format: " + ex.Message);
            }
        }

    }

    public class member_info()
    {
        public string username { get; set; }
        public string password { get; set; }
        public string fullname { get; set; }
        public string department { get; set; }
        public string q1 { get; set; }
        public string a1 { get; set; }

    }
}
