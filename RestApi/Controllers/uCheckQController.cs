using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;
using static RestAPI.Controllers.uLoginController;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class uCheckQController : Controller
    {

        [HttpPost]
        public async Task<dynamic> PostWithModelWithFromBody([FromBody] check_info model)
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
                    cmd = $"SELECT * FROM config_member WHERE username = '{model.username}'";   // AND department = '{model.department}'
                    dt = ClientsUnitsMySql.ExecuteWithQuery(cmd);

                    if (dt.Rows.Count > 0)
                    {
               
                        msgcode = "00000";
                        message = dt.Rows[0]["q1"].ToString();

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

    public class check_info()
    {
        public string username { get; set; }
   
    }
}
