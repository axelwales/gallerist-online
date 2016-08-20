using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class VisitorDisplayViewModel
    {
        public  Game Game { get; private set; }

        public int Collectors { get; private set; }
        public int Investors { get; private set; }
        public int Vips { get; private set; }

        public bool HasCollectorAction { get; private set; }
        public bool HasInvestorAction { get; private set; }
        public bool HasVipAction { get; private set; }

        public string CollectorLocation { get; private set; }
        public string InvestorLocation { get; private set; }
        public string VipLocation { get; private set; }

        public VisitorTicketType InvestorType { get { return VisitorTicketType.investor; } }
        public VisitorTicketType CollectorType { get { return VisitorTicketType.collector; } }
        public VisitorTicketType VipType { get { return VisitorTicketType.vip; } }

        public GameActionState FormAction { get; private set; }

        public VisitorDisplayViewModel(string userName, Game game, Player player, GameVisitorLocation location)
        {
            Game = game;

            PlayerColor color;
            if (player != null)
                color = player.Color;
            else
                color = PlayerColor.none;

            var visitors = game.VisitorByPlayerAndLocation(color, location);
            Investors = visitors.Where(v => v.Type == VisitorTicketType.investor).Count();
            Collectors = visitors.Where(v => v.Type == VisitorTicketType.collector).Count();
            Vips = visitors.Where(v => v.Type == VisitorTicketType.vip).Count();

            SetFormActionParameters(userName, game, color, location);
        }

        private void SetFormActionParameters(string userName, Game game, PlayerColor color, GameVisitorLocation location)
        {
            CollectorLocation = VisitorTicketType.collector.ToString() + ':' + location.ToString() + ':' + color.ToString();
            InvestorLocation = VisitorTicketType.investor.ToString() + ':' + location.ToString() + ':' + color.ToString();
            VipLocation = VisitorTicketType.vip.ToString() + ':' + location.ToString() + ':' + color.ToString();

            var nextStates = game.CurrentTurn.GetNextActions().Select(a => a.State);
            if( nextStates.Contains(GameActionState.MoveVisitorFromLobby) )
            {
                FormAction = GameActionState.MoveVisitorFromLobby;
                HasCollectorAction = FormHelper.HasActionForm(userName, game, FormAction, CollectorLocation);
                HasInvestorAction = FormHelper.HasActionForm(userName, game, FormAction, InvestorLocation);
                HasVipAction = FormHelper.HasActionForm(userName, game, FormAction, VipLocation);
            }

            else if (nextStates.Contains(GameActionState.SellChooseVisitor))
            {
                FormAction = GameActionState.SellChooseVisitor;
                HasCollectorAction = FormHelper.HasActionForm(userName, game, FormAction, CollectorLocation);
                HasInvestorAction = FormHelper.HasActionForm(userName, game, FormAction, InvestorLocation);
                HasVipAction = FormHelper.HasActionForm(userName, game, FormAction, VipLocation);
            }

            else
                FormAction = GameActionState.NoAction;
        }
    }
}