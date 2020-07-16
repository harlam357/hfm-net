
using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

using HFM.Core.Net;

namespace HFM.Core.Services
{
    public abstract class SendMailService
    {
        public static SendMailService Default { get; } = DefaultSendMailService.Instance;

        // ReSharper disable once EmptyConstructor
        protected SendMailService()
        {

        }

        /// <summary>
        /// Sends an e-mail message.
        /// </summary>
        public abstract void SendEmail(string mailFrom, string mailTo, string subject, string body,
            string host, int port, string username, string password, bool enableSsl);

        private const string EmailAddressPattern = @"^[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,6}$";

        /// <summary>
        /// Validates an email address.
        /// </summary>
        public static bool ValidateEmail(string emailAddress)
        {
            if (String.IsNullOrWhiteSpace(emailAddress)) return false;

            var validEmailAddress = new Regex(EmailAddressPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return validEmailAddress.IsMatch(emailAddress);
        }
    }

    public class DefaultSendMailService : SendMailService
    {
        public static DefaultSendMailService Instance { get; } = new DefaultSendMailService();

        protected DefaultSendMailService()
        {

        }

        /// <summary>
        /// Sends an e-mail message using an <see cref="SmtpClient"/>.
        /// </summary>
        public override void SendEmail(string mailFrom, string mailTo, string subject, string body,
            string host, int port, string username, string password, bool enableSsl)
        {
            using (var message = new MailMessage(mailFrom, mailTo, subject, body))
            {
                using (var client = new SmtpClient(host, port))
                {
                    client.Credentials = NetworkCredentialFactory.Create(username, password);
                    client.EnableSsl = enableSsl;
                    client.Send(message);
                }
            }
        }
    }

    public class NullSendMailService : SendMailService
    {
        public static NullSendMailService Instance { get; } = new NullSendMailService();

        protected NullSendMailService()
        {

        }

        public override void SendEmail(string mailFrom, string mailTo, string subject, string body,
            string host, int port, string username, string password, bool enableSsl)
        {
            // do nothing
        }
    }
}
