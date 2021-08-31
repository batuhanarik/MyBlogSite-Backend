using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlogAngularCore.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;

namespace MyBlogAngularCore.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HelperController : ControllerBase
    {

        [HttpPost]
        public IActionResult SendContactEmail(Contact contact)
        {
            try

            {

                MailMessage mailMessage = new MailMessage();



                SmtpClient smtpClient = new SmtpClient();



                mailMessage.From = new MailAddress("batuhanarik123@gmail.com");

                mailMessage.To.Add("batuhanarik123@gmail.com"); // kime gidicek email burada belirtiyoruz



                mailMessage.Subject = contact.Subject;

                mailMessage.Body = contact.Message;

                mailMessage.IsBodyHtml = true;

                mailMessage.Priority = MailPriority.High;

                smtpClient.Host = "smtp.gmail.com";

                smtpClient.Port = 587;

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtpClient.EnableSsl = true;

                smtpClient.UseDefaultCredentials = false;

                smtpClient.Credentials = new System.Net.NetworkCredential("batuhanarik123@gmail.com", "Simaybal39");

                smtpClient.EnableSsl = true;

                smtpClient.Send(mailMessage);

                return Ok();

            }

            catch (Exception ex)

            {

                return BadRequest(ex.Message);

            }


        }
    }
}
