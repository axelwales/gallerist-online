using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameControllerHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.GameLogic;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class ActionContextInvoker
    {
        ActionContext _context;
        public Game Game { get; set; }


        public ActionContextInvoker(Game game)
        {
            Game = game;
            _context = ActionContextFactory.GetContext(game.CurrentTurn.CurrentAction, game);
        }

        public bool DoAction(ActionRequest request)
        {
            var newAction = Game.CurrentTurn.GetPendingAction(request);
            if (!IsValidTransition(newAction))
            {
                return false;
            }
            //todo remove the pending action that corresponds to the new action request
            Game.CurrentTurn.RemoveAllSiblingActions(newAction);

            DoActionSingle(newAction);

            //todo make sure this executes all executable actions, not just the first.
            var nextActions = Game.CurrentTurn.GetNextActions();
            while (nextActions != null)
            {
                var nextAction = nextActions.First();
                if (nextAction.IsExecutable)
                {
                    Game.CurrentTurn.RemoveAllSiblingActions(nextAction);
                    DoActionSingle(nextAction);
                }
                else
                    break;
                nextActions = Game.CurrentTurn.GetNextActions();
            }
            return true;
        }

        public void DoActionSingle(GameAction action)
        {
            Game.CurrentTurn.SetCurrentAction(action);
            if (!_context.NameToState.ContainsKey(action.State))
            {
                _context = ActionContextFactory.GetContext(action.State, Game);
            }
            _context.DoAction(action);
            Game.CurrentTurn.AddCompletedAction(action);
        }

        public bool IsValidTransition(GameAction action)
        {
            if (Game.CurrentTurn.GetNextActions().Any(a => a.State == action.State))
            {
                return IsValidGameState(action);
            }
            return false;
        }

        public bool IsValidTransition(GameActionState state)
        {
            if (Game.CurrentTurn.GetNextActions().Any(a => a.State == state))
            {
                var action = new GameAction { State = state };
                return IsValidGameState(action);
            }
            return false;
        }

        public bool IsValidTransition(GameActionState state, string location)
        {
            if (Game.CurrentTurn.GetNextActions().Any(a => a.State == state))
            {
                var action = Game.CurrentTurn.GetNextActions().First(a => a.State == state);
                action.StateParams["Location"] = location;
                return IsValidGameState(action);
            }
            return false;
        }

        public bool IsValidGameState(GameAction action)
        {
            var context = ActionContextFactory.GetContext(action, Game);
            return context.IsValidGameState(action);
        }

    }
}