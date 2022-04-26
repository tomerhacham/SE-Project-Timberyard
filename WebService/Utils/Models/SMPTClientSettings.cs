using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Utils.Models
{
    public class SMPTClientSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string SenderAddress { get; set; }
    }
}
