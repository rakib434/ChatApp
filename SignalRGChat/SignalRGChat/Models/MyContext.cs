using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SignalRGChat.Models
{
    public class MyContext :DbContext
    {
        public DbSet<GroupMessage> groupMessages { get; set; }
        public DbSet<message> messages { get; set; }
    }
}