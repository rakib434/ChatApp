using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRGChat.Models;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Threading;

namespace SignalRGChat.Hubs
{
    public class MyHub : Hub
    {
        static int TotalUser;
        MyContext Db = new MyContext();
        //=================== Connection =====================
        public override Task OnConnected() // New User Connect Message + Time
        {
            var UserName = Context.User != null ? Context.User.Identity.Name : "Unknown@gmail.com";

            MailAddress addr = new MailAddress(UserName);
            string username = addr.User;
            
            //=========Connection Status===========
            Interlocked.Increment(ref TotalUser);
            Clients.All.NewUserConnected("<b style='color: cornflowerblue'>" + username.ToUpper() + "</b>  " + DateTime.Now.ToLongTimeString() + " Now: " + TotalUser);
            //=====================================

            //============ Online User=============
            Clients.All.OnlineUser(username);
            //=====================================

            LoadMessag();                   //Load From database
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            var UserName = Context.User != null ? Context.User.Identity.Name : "Unknown@gmail.com";

            MailAddress addr = new MailAddress(UserName);
            string username = addr.User;

            Interlocked.Decrement(ref TotalUser);
            Clients.All.NewUserConnected("<b style='color: red'> " + username.ToUpper() + "</b> ReConnected " + DateTime.Now.ToLongTimeString() + " Now: " + TotalUser);

            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var UserName = Context.User != null ? Context.User.Identity.Name : "Unknown@gmail.com";

            MailAddress addr = new MailAddress(UserName);
            string username = addr.User;
            
            //========= Connection Status ===========
            Interlocked.Decrement(ref TotalUser);
            Clients.All.NewUserConnected("<b style='color: red'> " + username.ToUpper() + "</b> " + DateTime.Now.ToLongTimeString() + " Now: " + TotalUser);
            //======================================

            //============ Online User =============
            Clients.All.OnlineDisconnectedUser(username);
            //=====================================

            return base.OnDisconnected(stopCalled);
        }
        

        //=================== Public Chat Message =========================
        public Task SendMassege(string user, string message)    //Openly Message To EveryOne
        {
            var UsrName = Context.User != null ? Context.User.Identity.Name : "Unknown@gmail.com";

            if (user == "" && UsrName == "Unknown@gmail.com")                                     //user force to insert message
            {
                user = "<b> Admin:</b> ";                       // if not set userName, userName fill with Admin 
                message = "<b style='color: red'>Please Enter User Name</b>"; // and message this
                Clients.Caller.SendToAllInGroup(user, message);
            }
            else if (message == "")
            {
                user = "<b> Admin:</b> ";
                message = "<b style='color: red'>you forget to write message!!</b>";
                Clients.Caller.SendToAllInGroup(user, message);
            }
            else if (!string.IsNullOrEmpty(message))            //Save To database
            {
                //var UsrName = Context.User != null ? Context.User.Identity.Name : "Unknown@gmail.com";
               
                    message m = new message()
                    {
                        Message = message,
                        PostDate = DateTime.Now,
                        UserName = UsrName
                    };

                    Db.messages.Add(m);
                    Db.SaveChangesAsync();
                    var time = DateTime.Now.ToShortTimeString();
                    //var userName = user == "" ? UsrName : user;
                    var uname = "";
                    if (Context.User != null)
                    {
                        MailAddress addr = new MailAddress(Context.User.Identity.Name);
                        uname = addr.User;
                    }

                    user = Context.User != null && user == "" ? uname : user;

                    Clients.All.SendToAll(user, message + " <b style='color: rgba(128, 128, 128, .4)'>" + time + "<b>");
                
            }
            return Clients.All.NoCall();
        }


        //===================== Group =========================
        public Task Join(string groupName)          //Create or Join Group
        {
            if (groupName != "" && groupName != null)
            {
                return Groups.Add(Context.ConnectionId, groupName);
            }
            return Clients.All.NoCall();
        }

        public Task Leave(string groupName)         //leave Group
        {
            var user = "<b style='color: Green'>Admin: </b>";
            var message = "You Leave from <b style='color: red'>" + groupName + "</b>";
            try
            {
                Groups.Remove(Context.ConnectionId, groupName);
            }
            catch (Exception)
            {
                message = "<b style='color: red'>Select a Group before you leave</b>";   // and message this
            }
            Clients.Caller.SendToAll(user, message);
            return Clients.Caller.SendToAllInGroup(user, message);
        }


        //=================== Message to a Group ===================
        public Task SendGroupMassege(string groupName, string user, string message) //Send Group Message to a group
        {
            try
            {
                if (string.IsNullOrEmpty(groupName))
                {
                    user = "<b> Admin:</b> ";                       // if not set userName, userName fill with Admin 
                    message = "<b style='color: red'>Please Join in a Group first</b>";   // and message this
                    Clients.Caller.SendToAllInGroup(user, message);
                }
                else if (user == "")
                {
                    user = "<b> Admin:</b> ";                        // if not set userName, userName fill with Admin 
                    message = "<b style='color: red'>Please Enter User Name</b>"; // and message this
                    Clients.Caller.SendToAllInGroup(user, message);
                }
                else if (message == "")
                {
                    user = "<b> Admin:</b> ";
                    message = "<b style='color: red'>you forget to write message!!</b>";
                    Clients.Caller.SendToAllInGroup(user, message);
                }
                else
                {
                    var UsrName = Context.User != null ? Context.User.Identity.Name : "Unknown@gmail.com";
                    GroupMessage Gm = new GroupMessage()
                    {
                        GroupName = groupName,
                        GrpMessage = message,
                        PostDate = DateTime.Now,
                        UserName = UsrName
                    };

                    Db.groupMessages.Add(Gm);
                    Db.SaveChangesAsync();

                    var fullMessage = message + "    <b style='color: rgba(128, 128, 128, .3)'>" + DateTime.Now.ToShortTimeString() + "</b>";
                    Clients.Group(groupName).SendToAllInGroup(user, fullMessage);
                }
            }
            catch (Exception)
            {
                Clients.All.SendToAllInGroup("<b> Admin:</b> ", "<b style='color: red'>Something went wrong!!!</b>"); // if no group select by user

            }

            return Clients.All.NoCall();                //No Call this

        }

        //===================== Load Public Chat message ============================
        private void LoadMessag()                               //load old message
        {
            var list = from m in Db.messages
                       orderby m.PostDate
                       select new
                       {
                           user = m.UserName,
                           msz = m.Message,
                           time = m.PostDate
                       };

            foreach (var i in list)
            {
                MailAddress addr = new MailAddress(i.user);
                string username = addr.User;
                var shorttime = i.time.ToShortTimeString();
                //string domain = addr.Host;
                Clients.Caller.OldMessage(username, i.msz, shorttime);
            }
        }

        //=================== Load Group Message ====================
        public void LoadGroupMessage(string GroupNames)
        {
            var Glist = from g in Db.groupMessages
                        orderby g.PostDate
                        where g.GroupName == GroupNames                //filter By GroupName
                        select new
                        {
                            gUser = g.UserName,
                            gMsz = g.GrpMessage,
                            gName = g.GroupName,
                            gDate = g.PostDate
                        };

            foreach (var List in Glist)
            {
                MailAddress addr = new MailAddress(List.gUser);
                string username = addr.User;                        //email address:: devide name and host

                var shorttime = List.gDate.ToShortTimeString();     //short time
                //string domain = addr.Host;

                Clients.Caller.GroupOldMessage(username, List.gMsz, shorttime);     //only new join member can see this
            }

        }

    }
}