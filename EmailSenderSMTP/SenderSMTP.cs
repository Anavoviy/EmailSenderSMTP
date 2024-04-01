using System.Net.Mail;
using System.Net;

namespace EmailSenderSMTP
{
    public class SenderSMTP
    {
        private string NameAuthor { get;  } 
        private string Host { get; }
        private int Port { get; }
        private string EmailFrom { get; }
        private string Pass { get; }
        private int EmailPerMinutes { get; }
        private SmtpClient SmtpClient { get; }


        public SenderSMTP(MailService mailService, string emailFrom, string pass, string nameAuthor = "", int emailsPerMinutes = 10)
        {
            Host = SetHost(mailService);
            Port = SetPort(mailService);
            EmailFrom = emailFrom;
            Pass = pass;
            EmailPerMinutes = emailsPerMinutes;
            NameAuthor = nameAuthor;


            // Настройка SmtpClient
            SmtpClient smtp = new SmtpClient(this.Host, this.Port);
            smtp.Credentials = new NetworkCredential(this.EmailFrom, this.Pass);
            smtp.EnableSsl = true;

            this.SmtpClient = smtp;
        }
        

        public MailMessage CreateMailMessageBodyIsHTML(string emailTo, string name = null, string subject = "", string bodyHTML = "")
        {
            var from = new MailAddress(this.EmailFrom, name);
            var to = new MailAddress(emailTo);
            var mail = new MailMessage(from, to);

            mail.Subject = subject;
            mail.Body = bodyHTML;
            mail.IsBodyHtml = true;

            return mail;
        }
        public MailMessage CreateMailMessageBodyIsText( string emailTo, string name = null, string subject = "", string bodyText = "")
        {
            var from = new MailAddress(this.EmailFrom);
            if (name == null)
                from = new MailAddress(this.EmailFrom, this.NameAuthor);
            else
                from = new MailAddress(this.EmailFrom, name);

            var to = new MailAddress(emailTo);
            var mail = new MailMessage(from, to);

            mail.Subject = subject;
            mail.Body = bodyText;

            return mail;
        }

        public void SendMail(MailMessage mail)
        {
            this.SmtpClient.Send(mail);
        }
        public void SendMail(string emailTo, string name = null, string subject = "", string bodyText = "")
        {
            MailMessage mail = this.CreateMailMessageBodyIsText(emailTo, name, subject, bodyText);
            this.SmtpClient.Send(mail);
        }

        public async Task SendMailAsync(MailMessage mail)
        {
            await this.SmtpClient.SendMailAsync(mail);
        }
        public async Task SendMailAsync(string emailTo, string name = null, string subject = "", string bodyText = "")
        {
            MailMessage mail = this.CreateMailMessageBodyIsText(emailTo, name, subject, bodyText);
            await this.SmtpClient.SendMailAsync(mail);
        }

        public void SendMailToAddresses(MailMessage message, IEnumerable<string> addresses)
        {
            try
            {
                List<MailMessage> messages = new List<MailMessage>();

                if (message.IsBodyHtml)
                    foreach (string address in addresses)
                        messages.Add(this.CreateMailMessageBodyIsHTML(address, message.From.DisplayName, message?.Subject, message?.Body));
                else
                    foreach (string address in addresses)
                        messages.Add(this.CreateMailMessageBodyIsText(address, message.From.DisplayName, message?.Subject, message?.Body));

                foreach (MailMessage oneMessage in messages)
                {
                    Thread.Sleep(60000 / this.EmailPerMinutes);
                    this.SendMail(oneMessage);
                }

            } 
            catch (Exception ex) { Logger.Log(ex.Message); }
        }
        public void SendMailsToAddresses(IEnumerable<MailMessage> mails)
        {
            try
            {
                if (mails == null) throw new ArgumentNullException();

                foreach (MailMessage mail in mails)
                {
                    Thread.Sleep(60000 / this.EmailPerMinutes);
                    this.SendMail(mail);
                }
            }
            catch (Exception ex) { Logger.Log(ex.Message); }
        }

        public async Task SendMailToAddressesAsync(MailMessage message, IEnumerable<string> addresses)
        {
            try
            {
                List<MailMessage> messages = new List<MailMessage>();

                if (message.IsBodyHtml)
                    foreach (string address in addresses)
                        messages.Add(this.CreateMailMessageBodyIsHTML(address, message.From.DisplayName, message?.Subject, message?.Body));
                else
                    foreach(string address in addresses)
                        messages.Add(this.CreateMailMessageBodyIsText(address, message.From.DisplayName, message?.Subject, message?.Body));

                foreach (MailMessage oneMessage in messages)
                {
                    Thread.Sleep(60000 / this.EmailPerMinutes);
                    await this.SendMailAsync(oneMessage);
                }
            }
            catch (Exception ex) { Logger.Log(ex.Message); }
        }
        public async Task SendMailsToAddressesAsync(IEnumerable<MailMessage> mails)
        {
            try
            {
                if (mails == null) throw new ArgumentNullException();

                foreach (MailMessage oneMessage in mails)
                {
                    Thread.Sleep(60000 / this.EmailPerMinutes);
                    await this.SendMailAsync(oneMessage);
                }
            }
            catch (Exception ex) { Logger.Log(ex.Message); }
        }

        private string SetHost(MailService mailService)
        {
            switch (mailService)
            {
                case MailService.MailRu:
                    return "smtp.mail.ru";
                    break;
                case MailService.YandexRu:
                    return "smtp.yandex.ru";
                    break;
                case MailService.PochtaRu:
                    return "smtp.pochta.ru";
                    break;
                case MailService.RamblerRu:
                    return "smtp.rambler.ru";
                    break;
                case MailService.GoogleCom:
                    return "smtp.gmail.com";
                    break;
                default:
                    throw new Exception("SMTP сервера для данного почтового сервиса не существует");
            }
        }
        private int SetPort(MailService mailService)
        {
            switch (mailService)
            {
                case MailService.MailRu:
                    return 25;
                    break;
                case MailService.YandexRu:
                    return 25;
                    break;
                case MailService.PochtaRu:
                    return 25;
                    break;
                case MailService.RamblerRu:
                    return 25;
                    break;
                case MailService.GoogleCom:
                    return 587;
                    break;
                default:
                    throw new Exception("SMTP сервера для данного почтового сервиса не существует");
            }
        }
    }
}