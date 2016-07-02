using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.GameLogic;

namespace TeamJAMiN.Models.GameViewHelpers
{
    public static class FormHelper
    {
        public static bool IsActivePlayer(string userName, Game game)
        {
            return game.CurrentPlayer.UserName == userName;
        }

        public static bool HasActionForm(string userName, Game game, GameActionState state, string location, bool otherConditions = true)
        {
            if(!otherConditions)
                return false;

            if (!IsActivePlayer(userName, game))
                return false;

            var request = new ActionRequest { State = state, ActionLocation = location };
            return ActionManager.IsValidTransition(request, game);
        }

        public static void GetActionForm(this HtmlHelper html, string partialViewPath, Game game, GameActionState state, string location, object model = null)
        {
            string partialView;
            if (model != null)
                partialView = html.Partial(partialViewPath,model).ToString();
            else
                partialView = html.Partial(partialViewPath).ToString();
            using (html.BeginForm("TakeGameAction", "Game", new { id = game.Id, gameAction = state, actionLocation = location }, FormMethod.Post, new { role = "form" }))
            {
                string inner = html.AntiForgeryToken().ToString() + InsertSubmitElement(partialView);
                html.ViewContext.Writer.Write(inner);
            }
        }

        public static string InsertSubmitElement(string partialView)
        {
            Regex r = new Regex("<div.*?>");
            string startTag = r.Match(partialView).ToString();
            string remainder = r.Replace(partialView, "", 1);
            return startTag + "<input type=\"submit\" class=\"action-button\" value=\"\" />" + remainder;
        }
    }
}