using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameControllerHelpers;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class ActionContextInvoker
    {
        ActionContext _context;
        public Game Game { get; set; }


        public ActionContextInvoker(Game game)
        {
            Game = game;
            _context = ActionContextFactory.GetContext(game.CurrentTurn.CurrentAction.State, game);
        }

        public bool DoAction(GameActionState state, string actionLocation = "")
        {
            var newAction = new GameAction { State = state, Location = actionLocation, IsExecutable = true };
            if (!IsValidTransition(newAction))
            {
                return false;
            }
            //todo remove the pending action that corresponds to the new action request
            Game.CurrentTurn.RemoveAllSiblingActions(newAction.State);

            DoActionSingle(newAction);

            var nextActions = Game.CurrentTurn.GetNextActions();
            if (nextActions.Count == 1)
            {
                var nextAction = nextActions.Single();
                if (nextAction.IsExecutable)
                {
                    Game.CurrentTurn.RemoveAllSiblingActions(nextAction);
                    DoActionSingle(nextAction);
                }
            }
            return true;
        }

        public void DoActionSingle(GameAction action)
        {
            Game.CurrentTurn.SetCurrentAction(action.State, action.Location);
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

        public bool IsValidGameState(GameAction action)
        {
            var context = ActionContextFactory.GetContext(action.State, Game);
            return context.IsValidGameState(action);
        }

    }
}