using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using TeamJAMiN.Controllers.Hubs.HubHelpers;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace TeamJAMiN.Controllers.Hubs
{
    public class GameActionHub : Hub
    {

        public void Activate()
        {

        }

        public void Update(string[] action)   //action = {[param1],[param2]}
        {
            Clients.Others.update(action[0], action[1]);
        }

        public void RefreshGame(List<string> userIds)
        {
            Clients.Users(userIds).refresh();
        }
    }
}