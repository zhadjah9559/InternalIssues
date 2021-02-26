using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


//This class deserializes the information inside the MailSettings object that was created inside the appsettings.json folder
namespace InternalIssues.Models
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
