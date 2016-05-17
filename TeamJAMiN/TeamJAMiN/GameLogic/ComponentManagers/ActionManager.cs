using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

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

        public static bool IsValidAction(GameActionState state, string actionLocation, Game game, GameAction parent = null)
        {
            var action = new GameAction { State = state, Location = actionLocation, Parent = parent };
            return action.IsValidAction(game);
        }

        public static bool IsValidTransition(this GameAction action, Game game)
        {
            var invoker = new ActionContextInvoker(game);
            return invoker.IsValidTransition(action);
        }

        public static bool IsValidTransition(GameActionState state, string actionLocation, Game game, GameAction parent = null)
        {
            var action = new GameAction { State = state, Location = actionLocation, Parent = parent };
            return action.IsValidTransition(game);
        }

        public static bool IsValidTicketBonus(GameActionState state, Game game, VisitorTicketType type, GameAction parent = null)
        {
            if (parent == null)
            {
                parent = game.CurrentTurn.CurrentAction.Parent;
            }
            var actionLocation = type.ToString();
            return IsValidTransition(state, actionLocation, game, parent);
        }

        public static bool IsValidTicketBonus(Game game, VisitorTicketType type)
        {
            var state = game.CurrentTurn.CurrentAction.State;
            return IsValidTicketBonus(state, game, type);
        }
    }
}