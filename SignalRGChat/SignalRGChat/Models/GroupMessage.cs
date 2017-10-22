using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRGChat.Models
{
    public class GroupMessage
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string GrpMessage { get; set; }
        public string GroupName { get; set; }
        public DateTime PostDate { get; set; }
    }
}