
using System.Net.Mail;

using HFM.Core.Net;

namespace HFM.Core.Services
{
    public class SendMailService
    {
        /// <summary>
        /// Sends an e-mail message.
        /// </summary>
        public void SendEmail(string mailFrom, string mailTo, string subject, string body, 
            string host, int port, string username, string password, bool enableSsl)
        {
            using (var message = new MailMessage(mailFrom, mailTo, subject, body))
            {
                var client = new SmtpClient(host, port)
                {
                    Credentials = NetworkCredentialFactory.Create(username, password),
                    EnableSsl = enableSsl
                };
                client.Send(message);
            }
        }
    }
}
