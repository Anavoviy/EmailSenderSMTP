using System.Net.Mail;
using System.Net;

namespace EmailSenderSMTP
{
    public class SenderSMTP
    {
        


        private string Host { get; }
        private int Port { get; }
        private string EmailFrom { get; }
        private string Pass { get; }
        private int EmailPerMinutes { get; }
        private SmtpClient SmtpClient { get; }


        public SenderSMTP(MailService mailService, string emailFrom, string pass, int emailsPerMinutes = 10)
        {
            Host = SetHost(mailService);
            Port = SetPort(mailService);
            EmailFrom = emailFrom;
            Pass = pass;
            EmailPerMinutes = emailsPerMinutes;


            // Настройка SmtpClient
            SmtpClient smtp = new SmtpClient(this.Host, this.Port);
            smtp.Credentials = new NetworkCredential(this.EmailFrom, this.Pass);
            smtp.EnableSsl = true;

            this.SmtpClient = smtp;
        }

        
        public MailMessage CreateMailMessageBodyIsHTML(string name, string emailTo, string subject = "", string body = "")
        {
            var from = new MailAddress(this.EmailFrom, name);
            var to = new MailAddress(emailTo);
            var mail = new MailMessage(from, to);

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            return mail;
        }
        public MailMessage CreateMailMessageBodyIsText(string name, string emailTo, string subject = "", string body = "")
        {
            var from = new MailAddress(this.EmailFrom, name);
            var to = new MailAddress(emailTo);
            var mail = new MailMessage(from, to);

            mail.Subject = subject;
            mail.Body = body;

            return mail;
        }

        public async void SendMail(MailMessage mail)
        {
            this.SmtpClient.Send(mail);
        }
        public void SendMailToAddresses(MailMessage message, IEnumerable<string> addresses)
        {
            try
            {
                long counter = 0;
                if (message.IsBodyHtml)
                    foreach (string address in addresses)
                    {
                        if (counter % EmailPerMinutes == 0)
                            Thread.Sleep(60000);

                        MailMessage mailMessage = this.CreateMailMessageBodyIsHTML(message.From.DisplayName, address, message?.Subject, message?.Body);
                        this.SendMail(mailMessage);

                        counter++;
                    }

            } 
            catch (Exception ex) { throw ex; }
        }
        public void SendMailsToAddresses(IEnumerable<MailMessage> mails)
        {
            try
            {
                if (mails == null) throw new ArgumentNullException();

                MailMessage[] mailMessages = mails.ToArray();


                for (int i = 0; i < mails.Count(); i++)
                    this.SendMail(mailMessages[i]);
            }
            catch (Exception ex) { throw ex; }
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