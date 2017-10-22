using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartupAttribute(typeof(SignalRGChat.Startup))]
namespace SignalRGChat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/Index")
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);


            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
