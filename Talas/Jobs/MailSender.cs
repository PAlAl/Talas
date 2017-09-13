using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Objects;
using Talas.Models;
using Talas.Objects;

namespace Talas.Jobs
{
    public static class MailSender
    {
        private class EmailMessage
        {
            public Int32 EngineId { get; set; }
            public String Receiver { get; set; }
            public String Message { get; set; }
            public Int32 NewEmailId { get; set; }

            public EmailMessage(int engineId, string receiver, string message, int newEmailId)
            {
                EngineId = engineId;
                Receiver = receiver;
                Message = message;
                NewEmailId = newEmailId;
            }
        }

        private static MailAddress _from = new MailAddress("joy_kirish@mail.ru", "Talas");
        private static MailAddress _to = new MailAddress("dtritus93@yandex.ru");
        private static String _emailFromLogin = "joy_kirish@mail.ru";
        private static String _emailFromPassword = "oxis82";
        public static void StartMailSenderWork()
        {
            Thread myThread = new Thread(new ThreadStart(Work));
            myThread.Start();
        }

        public static void Work()
        {
            while (true)
            {
                List<EmailMessage> sendMessages = GetNewEmails();
                if (sendMessages.Any())
                {
                    foreach (EmailMessage sendMessage in sendMessages)
                    {
                        SendMessage(sendMessage.Receiver, sendMessage.Message);
                    }

                    DeleteNewEmailsMessageByIds(sendMessages.Select(x => x.NewEmailId).ToArray());
                }
                Thread.Sleep(5000);
            }
        }

        private static void SendMessage(String toAddress, String message)
        {
            MailMessage m = new MailMessage(_from, new MailAddress(toAddress));
            m.Subject = "Talas";
            m.Body = message;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential(_emailFromLogin, _emailFromPassword);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(m);
        }

        private static List<EmailMessage> GetNewEmails()
        {
            List<EmailMessage> emailMessages = new List<EmailMessage>();
            using (AppContext db = new AppContext())
            {
                List<NewEmailsMessage> newEmails = db.NewEmailsMessage.ToList();
                if (newEmails.Any())
                {
                    List<Message> messages = db.Messages.ToList();

                    foreach (NewEmailsMessage newEmail in newEmails )
                    {
                        Message message = messages.FirstOrDefault(x => x.Id == newEmail.MessageId);

                        if (message == null)
                            continue;

                        EmailMessage existEmailsMessage = emailMessages.FirstOrDefault(x => x.EngineId == newEmail.EngineId);

                        if (existEmailsMessage == null)
                        {
                            Engine engine = db.Engines.Include("User").FirstOrDefault(x => x.Id == newEmail.EngineId);

                            if (engine == null || engine.User == null)
                                continue;

                            String userMail = engine.User.Email;

                            if (!String.IsNullOrEmpty(userMail))
                            {
                                emailMessages.Add(new EmailMessage(engine.Id, userMail, message.Text, newEmail.Id));
                            }
                        }
                        else
                        {
                            emailMessages.Add(new EmailMessage(newEmail.EngineId, existEmailsMessage.Receiver, message.Text, newEmail.Id));
                        }
                    }
                }
            }

            return emailMessages;
        }

        private static void DeleteNewEmailsMessageByIds(Int32[] ids)
        {
            using (AppContext db = new AppContext())
            {
                NewEmailsMessage[] delEmailsMessages = db.NewEmailsMessage.Where(x => ids.Contains(x.Id)).ToArray();
                db.NewEmailsMessage.RemoveRange(delEmailsMessages);
                db.SaveChanges();
            }
        }
    }
}