using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Models.GameViewHelpers
{
    public static class FormHelper
    {
        public static bool IsActivePlayer(string userName, Game game)
        {
            return game.CurrentPlayer.UserName == userName;
        }

        public static void GetActionForm(this HtmlHelper Html, string partialViewPath, Game game, GameActionState state, string location, object model = null)
        {
            string partialView;
            if (model != null)
                partialView = Html.Partial(partialViewPath,model).ToString();
            else
                partialView = Html.Partial(partialViewPath).ToString();
            using (Html.BeginForm("TakeGameAction", "Game", new { id = game.Id, gameAction = state, actionLocation = location }, FormMethod.Post, new { role = "form" }))
            {
                string inner = Html.AntiForgeryToken().ToString() + InsertSubmitElement(partialView);
                Html.ViewContext.Writer.Write(inner);
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