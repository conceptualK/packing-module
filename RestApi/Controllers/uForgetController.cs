using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;
using static RestAPI.Controllers.uLoginController;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class uForgetController : Controller
    {

        [HttpPost]
        public async Task<dynamic> PostWithModelWithFromBody([FromBody] forget_info model)
        {
            try
            {
                DataTable dt = new DataTable();
                string cmd = string.Empty;
                string msgcode = "00000";
                string message = string.Empty;
                string txtResponse = string.Empty;

                //  string password = model.password;

                try
                {
                    cmd = $"SELECT * FROM config_member WHERE username = '{model.username}' AND a1 = '{model.a1}'";   // AND department = '{model.department}'
                    dt = ClientsUnitsMySql.ExecuteWithQuery(cmd);

                    if (dt.Rows.Count > 0)
                    {
                        string password = model.password;
                        string hashedPassword = PasswordHasher.HashPassword(password);

                        cmd = $"UPDATE config_member set password = '{hashedPassword}' WHERE id = '{dt.Rows[0]["id"].ToString()}'";
                        ClientsUnitsMySql.ExecuteWithNoneQuery(cmd);
                        msgcode = "00000";
                        message = "OK, Update successfully";

                    }
                    else
                    {
                        msgcode = "00001";
                        message = "NG, Wrong in formation.";
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

    public class forget_info()
    {
        public string username { get; set; }
        public string password { get; set; }
        public string a1 { get; set; }

    }
}
