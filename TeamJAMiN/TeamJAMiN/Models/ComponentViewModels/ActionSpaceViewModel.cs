using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class ActionSpaceViewModel
    {
        public ActionSpaceViewModel(string userName, Game game, PlayerLocation location)
        {
            Game = game;
            IsActivePlayer = FormHelper.IsActivePlayer(userName, game);
            IsValidSpace = ValidateSpace(game, location);

            Location = location;
            State = (GameActionState)Enum.Parse(typeof(GameActionState), location.ToString());
            LocationName = HtmlNames[location][0];
            FirstActionName = HtmlNames[location][1];
            SecondActionName = HtmlNames[location][2];

            Color = game.Players.First(p => p.UserName == userName).Color;
            SetGalleristColorClass(game, location);
            SetPushAndPullByLocation(location);
        }

        public static Dictionary<PlayerLocation, string[]> HtmlNames = new Dictionary<PlayerLocation, string[]>
        {
            {PlayerLocation.ArtistColony, new string[] { "artist-colony", "discover", "buy"} },
            {PlayerLocation.MediaCenter, new string[] { "media-center", "promote", "hire"} },
            {PlayerLocation.InternationalMarket, new string[] { "international-market", "reputation", "auction"} },
            {PlayerLocation.SalesOffice, new string[] { "sales-office", "contract", "sell"} }
        };                                                          //used to get html friendly names to use in id fields, format: { (key) location, (value) { location name, first action, second action }

        public Game Game { get; private set; }

        public bool IsActivePlayer { get; private set; }
        public bool IsValidSpace { get; private set; }
        public string Highlight { get; private set; }

        public PlayerLocation Location { get; private set; }
        public GameActionState State { get; private set; }
        public string LocationName { get; private set; }            //html friendly name for location action space
        public string FirstActionName { get; private set; }     //html friendly name for the first action at a space
        public string SecondActionName { get; private set; }    //html friendly name for the second action at a space

        private PlayerColor Color { get; set; }
        public string GalleristColorClass { get; set; }             //css class for gallerist color

        public string Push { get; private set; }
        public string Pull { get; private set; }

        private void SetPushAndPullByLocation(PlayerLocation location)
        {
            Push = "";
            Pull = "";

            if (location == PlayerLocation.ArtistColony || location == PlayerLocation.MediaCenter)
            {
                Push = "col-xs-push-6";
                Pull = "col-xs-pull-6";
            }
        }

        private bool ValidateSpace(Game game, PlayerLocation location)
        {
            var state = (GameActionState)Enum.Parse(typeof(GameActionState), location.ToString());
            var action = new GameAction { State = state };
            action.StateParams["Location"] = location.ToString();
            var invoker = new ActionContextInvoker(game);
            return invoker.IsValidTransition(action);
        }

        private void SetGalleristColorClass(Game game, PlayerLocation location)
        {
            var playerAtLocation = game.Players.FirstOrDefault(p => p.GalleristLocation == location);
            if (playerAtLocation != null)
            {
                GalleristColorClass = playerAtLocation.Color.ToString() + "-gallerist";
            }
            else
            {
                GalleristColorClass = "";
            }
        }
    }
}