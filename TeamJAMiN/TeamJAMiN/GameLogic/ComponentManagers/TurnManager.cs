using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameControllerHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.GameLogic;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class TurnManager
    {
        public static void SetCurrentAction(this GameTurn turn, GameAction action)
        {
            if (turn.CurrentAction != null)
            {
                turn.AddCompletedAction(turn.CurrentAction);
            }
            turn.CurrentAction = action;
        }

        public static void AddPendingAction(this GameTurn turn, GameAction action)
        {
            turn.AddPendingAction(action, PendingPosition.first);
        }

        public static void AddPendingAction(this GameTurn turn, GameAction action, PendingPosition position)
        {
            action.Turn = turn;
            var toAdd = new List<GameAction> { action };
            AddPendingActions(turn, toAdd, position);
        }

        public static void AddPendingActions(this GameTurn turn, List<GameActionState> states, GameActionStatus status, bool isExecutable)
        {
            turn.AddPendingActions(states, null, status, PendingPosition.first, isExecutable);
        }

        public static void AddPendingActions(this GameTurn turn, List<GameActionState> states, GameAction parent, GameActionStatus status, PendingPosition position, bool isExecutable)
        {
            var newActions = new List<GameAction>();
            foreach (GameActionState state in states)
            {
                var action = new GameAction { State = state, Status = status, IsExecutable = isExecutable, Parent = parent, Turn = turn };
                newActions.Add(action);
            }
            AddPendingActions(turn, newActions, position);
        }

        public static void AddPendingActions(GameTurn turn, List<GameAction> newActions, PendingPosition position)
        {
            if (position == PendingPosition.first)
            {
                turn.PendingActions.Add(newActions);
            }
            else
            {
                turn.PendingActions.AddLast(newActions);
            }
        }

        public static void RemovePendingAction(this GameTurn turn, GameAction action)
        {
            turn.PendingActions.Remove(action);
        }

        public static void RemoveAllSiblingActions(this GameTurn turn, GameAction action)
        {
            var pending = turn.PendingActions;
            if (action != null)
            {
                if (action.Status == GameActionStatus.OptionalExclusive)
                {
                    var toRemove = pending.FirstOrDefaultList(a => a.State == action.State);
                    if(toRemove != null)
                        pending.Remove(toRemove);
                }
                else
                {
                    pending.Remove(action);
                }
            }
        }

        public static List<GameAction> GetNextActions(this GameTurn turn)
        {
            if (turn.PendingActions.Count > 0)
            {
                return turn.PendingActions.First;
            }
            return null;
        }

        public static GameAction GetPendingAction(this GameTurn turn, ActionRequest request)
        {
            var action = turn.PendingActions.FirstOrDefault(a => a.State == request.State);
            if(action == null)
            {
                action = new GameAction { State = request.State };
            }
            action.StateParams["Location"] = request.ActionLocation;

            return action;
        }

        public static void AddCompletedAction(this GameTurn turn, GameAction action)
        {
            if(action.IsComplete == false)
            {
                action.IsComplete = true;
                turn.CompletedActions.Add(action);
            }
        }

        public static void SetupFirstTurn(this Game newGame)
        {
            var firstTurn = new GameTurn { TurnNumber = 0, Type = GameTurnType.Setup, CurrentAction = new GameAction { State = GameActionState.GameStart } };
            newGame.Turns.Add(firstTurn);
            var next = new GameAction { State = GameActionState.Pass };
            (new ActionContextInvoker(newGame)).DoActionSingle(next);

        }
        public static void SetupNextTurn(this Game game)
        {
            var oldTurn = game.CurrentTurn;
            if(oldTurn.KickedOutPlayer != null)
            {
                game.SetupKickedOutTurn(oldTurn);
            }
            else
            {
                game.SetupTurn(oldTurn);
            }
        }

        public static void SetupKickedOutTurn(this Game game, GameTurn oldTurn)
        {
            //todo add turnstart action state, then add completed turn start action
            //todo add chooselocation to pending actions
            var newTurn = new GameTurn { Game = game, TurnNumber = oldTurn.TurnNumber + 1, CurrentPlayer = oldTurn.KickedOutPlayer };
            game.CurrentPlayerId = oldTurn.KickedOutPlayer.Id;
            game.Turns.Add(newTurn);
            newTurn.StartTurn();
        }

        public static void SetupTurn(this Game game, GameTurn oldTurn)
        {
            game.SetNextPlayer();
            var newTurn = new GameTurn { Game = game, TurnNumber = oldTurn.TurnNumber + 1, CurrentPlayer = game.CurrentPlayer };
            game.Turns.Add(newTurn);
            newTurn.StartTurn();
        }

        public static void StartTurn(this GameTurn turn)
        {
            var startAction = new GameAction { State = GameActionState.ChooseLocation };
            turn.CurrentAction = startAction;
            var invoker = new ActionContextInvoker(turn.Game);
            invoker.DoActionSingle(startAction);
        }
    }
}