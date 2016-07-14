using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class AuctionLocationViewModel
    {
        public Game Game { get; private set; }

        public string ActionLocation { get; private set; }
        public GameActionState State = GameActionState.Auction;

        public bool IsValidActionState { get; private set; }

        private string Column { get; set; }
        private string Row { get; set; }

        public string AuctionClass { get; private set; }
        public string AssistantCss { get; private set; }

        public AuctionLocationViewModel(string userName, Game game, string column, string row)
        {
            Game = game;
            Column = column;
            Row = row;
            ActionLocation = row + ':' + column;
            AuctionClass = AssistantCss = "";

            IsValidActionState = FormHelper.HasActionForm(userName, game, GameActionState.Auction, ActionLocation);

            var assistant = AssistantManager.GetAssistantByIMLocation(game, row, column);
            if (assistant != null)
                AssistantCss = "im-assistant-icon player-assistant-" + assistant.Player.Color.ToString();
            else
                SetAuctionClass(ActionLocation);
        }

        private void SetAuctionClass(string actionLocation)
        {
            var bonus = AuctionManager.GetBonusByAuctionLocationString(actionLocation);
            AuctionClass = IconCss.BonusClass[bonus];
        }
    }
}