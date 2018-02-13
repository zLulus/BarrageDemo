using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BarrageDemo2.Website.Connections
{
    public class BarrageConnection : PersistentConnection
    {
        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            //在这里可以做自己的业务处理，比如把弹幕存进redis等等
            return Connection.Broadcast(data, connectionId);
        }
    }
}