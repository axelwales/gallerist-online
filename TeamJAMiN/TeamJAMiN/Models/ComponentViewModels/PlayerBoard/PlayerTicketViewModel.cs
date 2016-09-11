using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels.PlayerBoard
{
    public class PlayerTicketViewModel
    {
        public Player Player { get; private set; }

        public VisitorTicketType Type { get; private set; }
        public int TicketCount { get; private set; }

        public bool HasActionForm { get; private set; }
        public GameActionState State { get { return GameActionState.ChooseTicketToSpend; } }
        public string Location { get { return Type.ToString(); } }

        public PlayerTicketViewModel(string userName, Player player, VisitorTicketType type)
        {
            Type = type;
            TicketCount = TicketManager.GetPlayerTicketCountByType(player, type);

            bool isPlayerBoardOfActivePlayer = player.Id == player.Game.CurrentPlayer.Id;
            HasActionForm = FormHelper.HasActionForm(userName, player.Game, State, Location, isPlayerBoardOfActivePlayer);
        }
    }
}