using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW.ClientAPI.Models
{
    public class EmailSettingsModel
    {
        public string SmtpHost { get; set; }
        public int Port { get; set; }

        public string MailAddress { get; set; }
        public string UserName { get; set; }
        public string  Password { get; set; }  
        public string EmailRecipients { get; set; }
        public string CCEmailRecipients { get; set; }
    }
}
