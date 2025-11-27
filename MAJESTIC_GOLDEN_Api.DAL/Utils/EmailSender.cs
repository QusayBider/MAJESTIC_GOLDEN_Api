using Azure.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.Utils
{
    public class EmailSender: IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("bdyrqsy32@gmail.com", "pyhr dijx hkcs mubr")
            };

            return client.SendMailAsync(
                new MailMessage(from: "bdyrqsy32@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                {IsBodyHtml=true}
                );
        }
    }
}
