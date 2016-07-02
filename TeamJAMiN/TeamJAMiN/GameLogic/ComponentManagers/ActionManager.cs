using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.GameLogic;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public enum PendingPosition
    {
        first, last
    }
    public static class ActionManager
    {

        public static GameAction GetTicketAction(VisitorTicketType type)
        {
            GameAction action = new GameAction();
            switch (type)
            {
                case VisitorTicketType.collector:
                    action.State = GameActionState.GetTicketCollector;
                    break;
                case VisitorTicketType.vip:
                    action.State = GameActionState.GetTicketVip;
                    break;
                case VisitorTicketType.investor:
                    action.State = GameActionState.GetTicketInvestor;
                    break;
                default:
                    action = null;
                    break;
            }
            return action;
        }

        public static bool IsValidAction(this GameAction action, Game game)
        {
            var invoker = new ActionContextInvoker(game);
            return invoker.IsValidGameState(action);
        }

        public static bool IsValidTransition(this GameAction action, Game game)
        {
            var invoker = new ActionContextInvoker(game);
            return invoker.IsValidTransition(action);
        }

        public static bool IsValidTransition(ActionRequest request, Game game)
        {
            var action = TurnManager.GetPendingAction(game.CurrentTurn, request);
            return action.IsValidTransition(game);
        }

        public static bool IsValidTicketBonus(Game game, VisitorTicketType type)
        {
            var nextAction = game.CurrentTurn.GetNextActions().FirstOrDefault();
            if (nextAction == null)
                return false;
            switch(nextAction.State)
            {
                case GameActionState.ChooseTicketAny:
                case GameActionState.ChooseTicketAnyTwo:
                case GameActionState.ChooseTicketCollectorInvestor:
                case GameActionState.ChooseTicketCollectorVip:
                case GameActionState.ChooseTicketToThrowAway:
                    break;
                default:
                    return false;
            }
            var action = (GameAction)nextAction.Clone();
            action.StateParams["Location"] = type.ToString();
            return IsValidTransition(action, game);
        }
    }
}