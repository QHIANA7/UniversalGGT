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
            String ConnectionString = ConfigurationManager.ConnectionStrings["defaultConnection"]?.ConnectionString;
            if(String.IsNullOrWhiteSpace(ConnectionString))
            {
                ConnectionString = "Data Source=tcp:ggtsvr.database.windows.net,1433;Initial Catalog=GGTDB;User Id=admin2013@ggtsvr.database.windows.net;Password=P@ssw0rd;";
            }
            Singleton<DatabaseService>.Instance.ConnectionString = ConnectionString;
        }
    }
}