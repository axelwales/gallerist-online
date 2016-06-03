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

        public static bool IsValidTransition(this GameAction action, Game game, bool getParent = false)
        {
            if(getParent)
            {
                var actions = game.CurrentTurn.GetNextActions();
                if( actions != null )
                {
                    var pendingAction = actions.FirstOrDefault(a => a.State == action.State);
                    if ( pendingAction != null )
                        action.Parent = pendingAction.Parent;
                }
            }
            var invoker = new ActionContextInvoker(game);
            return invoker.IsValidTransition(action);
        }

        public static bool IsValidTransition(GameActionState state, string actionLocation, Game game, GameAction parent = null, bool getParent = false)
        {
            var action = new GameAction { State = state, Location = actionLocation, Parent = parent };
            return action.IsValidTransition(game, getParent);
        }

        public static bool IsValidTicketBonus(GameActionState state, Game game, VisitorTicketType type, GameAction parent = null)
        {
            var actionLocation = type.ToString();
            return IsValidTransition(state, actionLocation, game, parent);
        }

        public static bool IsValidTicketBonus(Game game, VisitorTicketType type)
        {
            var nextAction = game.CurrentTurn.GetNextActions().FirstOrDefault();
            if (nextAction == null)
                return false;
            var state = nextAction.State;
            var parent = nextAction.Parent;
            switch(state)
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
            return IsValidTicketBonus(state, game, type, parent);
        }

        public static void SetActionId(this GameTurn turn, GameAction action)
        {
            action.Id = turn.NextActionId++;
        }
    }
}