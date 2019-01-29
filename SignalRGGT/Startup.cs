using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using SignalRGGT.Helpers;
using SignalRGGT.Services;
using System;
using System.Configuration;

[assembly: OwinStartup(typeof(SignalRGGT.Startup))]

namespace SignalRGGT
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
            Singleton<DatabaseService>.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString;
            
        }
    }
}