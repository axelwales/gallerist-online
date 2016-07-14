using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class ChooseTicketViewModel
    {
        public bool IsActivePlayer { get; private set; }
        public bool IsValidActionState { get; private set; }

        public VisitorTicketType Type { get; set; }
        public GameActionState State { get; set; }
        public string TicketClass { get; set; }

        public ChooseTicketViewModel(string userName, Game game, VisitorTicketType type)
        {
            IsActivePlayer = FormHelper.IsActivePlayer(userName, game);
            IsValidActionState = 
                game.CurrentTurn.CurrentAction.State == GameActionState.ChooseTicketAny ||
                game.CurrentTurn.CurrentAction.State == GameActionState.ChooseTicketAnyTwo;

        }
    }
}