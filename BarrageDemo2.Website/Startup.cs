using BarrageDemo2.Website.Connections;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(BarrageDemo2.Website.Startup))]
namespace BarrageDemo2.Website
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR<BarrageConnection>("/barrageConnection");
        }
    }
}