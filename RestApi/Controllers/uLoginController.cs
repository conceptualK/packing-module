using Aspose.Cells;
using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class uLoginController : Controller
    {

        [HttpPost]
        public async Task<dynamic> PostWithModelWithFromBody([FromBody] login_info model)
        {
            try
            {
                DataTable dt = new DataTable();
                string cmd = string.Empty;
                string msgcode = "00000";
                string message = string.Empty;
                string txtResponse = string.Empty;

                string password = model.password;
                string name = string.Empty; 
                string department = string.Empty;   
                try
                {
                    cmd = $"SELECT * FROM config_member WHERE username = '{model.username}'";
                    dt = ClientsUnitsMySql.ExecuteWithQuery(cmd);
                    bool isMatch = PasswordHasher.VerifyPassword(password, dt.Rows[0]["password"].ToString());
                   // Debug.WriteLine("Password Match: " + isMatch);
                     name = dt.Rows[0]["fullname"].ToString().Replace(" ","|");

                    string department_q = dt.Rows[0]["department"].ToString();

                    cmd = $"SELECT DEPT_ID,DEPT_NAME FROM SAJET.SYS_DEPT WHERE DEPT_ID ='{department_q}' AND ENABLED = 'Y' ORDER BY DEPT_NAME ASC ";
                    dt = ClientsUnitsOracle.ExecuteWithQuery(cmd);
                    department = dt.Rows[0]["DEPT_NAME"].ToString();

                    if (!isMatch)
                    {
                        msgcode = "00001";
                        message = "NG, Username or Password is incorrect !!";
                    }
                    else
                    {
                        msgcode = "00000";
                        message = "OK, Login successfully";
                    }

                }
                catch
                {
                    msgcode = "00001";
                    message = "NG, Error in Database";
                }

                txtResponse = "{\r\n\"MsgResult\":\"" + msgcode + "\",\r\n\"ErrorMessage\":\"" + " " + message +
                                " \",\r\n\"name\":\"" + name.Trim() + " \",\r\n\"department\":\"" + " " + department + "\"\r\n}";


                return Ok(txtResponse);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid JSON format: " + ex.Message);
            }
        }



        public class login_info()
        {
            public string username { get; set; }
            public string password { get; set; }
          
        }
    }
}
