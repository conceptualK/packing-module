using Microsoft.AspNetCore.Mvc;
using RestAPI.ExternalClass;
using System.Data;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class uPwdResetController : Controller
    {

        [HttpPost]
        public async Task<dynamic> PostWithModelWithFromBody([FromBody] rpwd_info model)
        {
            try
            {
                DataTable dt = new DataTable();
                string cmd = string.Empty;
                string msgcode = "00000";
                string message = string.Empty;
                string txtResponse = string.Empty;

                try
                {
                   
                        string password = model.msg;
                        string hashedPassword = PasswordHasher.HashPassword(password);

                        msgcode = "00000";
                        message = hashedPassword;             
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

    public class rpwd_info()
    {
        public string msg { get; set; }


    }
}
