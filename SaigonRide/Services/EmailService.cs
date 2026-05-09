using System.Net;
using System.Net.Mail;

namespace SaigonRide.Services
{
    public class EmailService
    {
        private const string FromEmail = "student399799@gmail.com";
        private const string FromPassword = "zzmd hsyv whus jygo";

        public void Send(string toEmail, string subject, string body)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(FromEmail, FromPassword)
                };

                using var message = new MailMessage(
                    new MailAddress(FromEmail, "SaigonRide"),
                    new MailAddress(toEmail))
                {
                    Subject = subject,
                    Body = body
                };

                smtp.Send(message);
            }
            catch { }
        }
    }
}