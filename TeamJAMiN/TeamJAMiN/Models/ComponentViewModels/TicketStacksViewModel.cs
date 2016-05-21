using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class TicketStacksViewModel
    {
        public int GameId { get; private set; }

        public List<TicketStackViewModel> Stacks { get; private set; }
        public List<VisitorTicketType> StackOrder = new List<VisitorTicketType> { VisitorTicketType.vip, VisitorTicketType.investor, VisitorTicketType.collector };

        public TicketStacksViewModel(string userName, Game game)
        {
            GameId = game.Id;

            Stacks = new List<TicketStackViewModel>();
            foreach( VisitorTicketType type in StackOrder )
            {
                var ticketStack = new TicketStackViewModel(userName, game, type);
                Stacks.Add(ticketStack);
            }
        }
    }

    public class TicketStackViewModel
    {
        public bool IsActivePlayer { get; private set; }
        public bool IsValidActionState { get; private set; }

        public GameActionState State { get; private set; }
        public string ActionLocation { get; private set; }

        public string TicketCssClass { get; private set; }
        public int AvailableTickets { get; private set; }
        
        public TicketStackViewModel(string userName, Game game, VisitorTicketType type)
        {
            IsActivePlayer = FormHelper.IsActivePlayer(userName, game);
            IsValidActionState = ActionManager.IsValidTicketBonus(game, type);
            if (IsValidActionState)
            {
                var nextAction = game.CurrentTurn.GetNextActions().FirstOrDefault();
                State = nextAction
                    .State;
                ActionLocation = type.ToString();
            }
            else
            {
                State = GameActionState.ChooseTicketAny; //this is a dummy state as this variable should not be used if IsValidActionState is false.
                ActionLocation = "";
            }

            TicketCssClass = "ticket-" + type.ToString().ToLower();
            SetAvailableTickets(game, type);
        }

        private void SetAvailableTickets(Game game, VisitorTicketType type)
        {
            switch(type)
            {
                case VisitorTicketType.collector:
                    AvailableTickets = game.AvailableCollectorTickets;
                    break;
                case VisitorTicketType.vip:
                    AvailableTickets = game.AvailableVipTickets;
                    break;
                case VisitorTicketType.investor:
                    AvailableTickets = game.AvailableInvestorTickets;
                    break;
                default:
                    break;
            }
        }

    }
}