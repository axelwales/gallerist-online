using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamJAMiN.Controllers.Hubs.HubHelpers
{
    public class PushHelper
    {
        private static PushHelper pushEngine = new PushHelper();

        private PushHelper()
        {

        }

        private IHubContext GetHub<T>()
            where T : IHub
        {
            return GlobalHost.ConnectionManager.GetHubContext<T>();
        }

        public static PushHelper GetPushEngine()
        {
            return pushEngine;
        }

        public void UpdateMyGamesList(List<string> userIds, string linkUrl, int gameId)
        {
            var hubContext = GetHub<GameListHub>();
            hubContext.Clients.Users(userIds).updateListItem(linkUrl, gameId);
        }

        public void RefreshGame(List<string> userIds)
        {
            var hubContext = GetHub<GameActionHub>();
            hubContext.Clients.Users(userIds).refresh();
        }

        public void RefreshGameList(List<string> userIds)
        {
            var hubContext = GetHub<GameListHub>();
            hubContext.Clients.Users(userIds).refresh();
        }
    }
}