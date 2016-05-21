using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TeamJAMiN.DataContexts;
using TeamJAMiN.Models;

namespace TeamJAMiN.Controllers.GameControllerHelpers
{
    public class AuthorizationHelpers
    {
        public static int GetGameIdFromRouteData(RouteData routeData)
        {
            var gameIdString = routeData.Values["id"].ToString();

            if (string.IsNullOrWhiteSpace(gameIdString)) { return -1; }

            int gameId = -1;
            int.TryParse(gameIdString, out gameId);

            return gameId;
        }

        public static bool IsUserInGame(string username, int gameId)
        {
            var isPlayerInGame = false;

            using (var galleristContext = new GalleristComponentsDbContext())
            {
                var game = galleristContext.Games.SingleOrDefault(m => m.Id == gameId);
                if (game == null) return false;

                using (var identityContext = new ApplicationDbContext())
                {
                    var currentUser = identityContext.Users.FirstOrDefault(m => m.UserName == username);
                    if (currentUser == null) return false;

                    foreach (var player in game.Players)
                    {
                        if (currentUser.Id == player.UserId)
                        {
                            isPlayerInGame = true;
                        }
                    }
                }
            }
            return isPlayerInGame;
        }

        public static bool IsUserHostOfGame(string username, int gameId)
        {
            var isPlayerHost = false;

            using (var galleristContext = new GalleristComponentsDbContext())
            {
                var game = galleristContext.Games.SingleOrDefault(m => m.Id == gameId);
                if (game == null) return false;

                using (var identityContext = new ApplicationDbContext())
                {
                    var currentUser = identityContext.Users.FirstOrDefault(m => m.UserName == username);
                    if (currentUser == null) return false;

                    foreach (var player in game.Players)
                    {
                        if (currentUser.Id == player.UserId && player.IsHost)
                        {
                            isPlayerHost = true;
                            break;
                        }
                    }
                }
            }
            return isPlayerHost;
        }

        public static bool IsUsersTurnInGame(string username, int gameId)
        {
            //todo revisit kicked out actions: how will we determine if the kicked out player's turn has started yet. Does that matter? Need to look into game rules.
            var isUsersTurn = false;

            using (var galleristContext = new GalleristComponentsDbContext())
            {
                var game = galleristContext.Games.SingleOrDefault(m => m.Id == gameId);
                if (game == null) return false;

                using (var identityContext = new ApplicationDbContext())
                {
                    var currentUser = identityContext.Users.FirstOrDefault(m => m.UserName == username);
                    if (currentUser == null) return false;

                    if (currentUser.Id == game.CurrentPlayer.UserId)
                    {
                        isUsersTurn = true;
                    }
                    else //check kicked out action
                    {
                        var kickedOutPlayerUser = game.Players.SingleOrDefault(m => m.Id == game.KickedOutPlayerId);
                        if (kickedOutPlayerUser != null)
                        {
                            if (currentUser.Id == kickedOutPlayerUser.UserId)
                            {
                                isUsersTurn = true;
                            }
                        }   
                    }
                }
            }
            return isUsersTurn;
        }
    }

    public class HttpForbiddenResult : HttpStatusCodeResult
    {
        public HttpForbiddenResult()
            : this(null)
        {
        }

        public HttpForbiddenResult(string statusDescription)
            : base(HttpStatusCode.Forbidden, statusDescription)
        {
        }
    }

    public class AuthorizePlayerInGame : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized) { return false; }

            var gameId = AuthorizationHelpers.GetGameIdFromRouteData(httpContext.Request.RequestContext.RouteData);
            if (gameId < 1) { return false; }

            var user = httpContext.User;
            return AuthorizationHelpers.IsUserInGame(user.Identity.Name, gameId);
        }        
    }

    public class AuthorizeHostOfGame : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized) return false;

            var gameId = AuthorizationHelpers.GetGameIdFromRouteData(httpContext.Request.RequestContext.RouteData);
            if (gameId < 1) { return false; }

            var user = httpContext.User;
            return AuthorizationHelpers.IsUserHostOfGame(user.Identity.Name, gameId);
        }
    }

    public class AuthorizeTurnInGame : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized) return false;

            var gameId = AuthorizationHelpers.GetGameIdFromRouteData(httpContext.Request.RequestContext.RouteData);
            if (gameId < 1) { return false; }

            var user = httpContext.User;

            return AuthorizationHelpers.IsUsersTurnInGame(user.Identity.Name, gameId);
        }

        //todo: currently when these fail they will redirect to login, which makes no sense in these cases. We want to either take them to a custom error page
        //or even better is to just send a 403 forbidden request. This override below may handle this but it has not been tested so I am leaving it commented for now
        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    if (filterContext.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        filterContext.Result = new HttpForbiddenResult();
        //    }
        //    else
        //    {
        //        filterContext.Result = new HttpUnauthorizedResult();
        //    }
        //}
    }
}