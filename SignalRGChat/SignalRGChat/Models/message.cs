using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRGChat.Models
{
    public class message
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime PostDate { get; set; }
    }
}