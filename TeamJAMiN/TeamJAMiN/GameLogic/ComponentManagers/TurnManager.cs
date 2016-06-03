using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameControllerHelpers;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class TurnManager
    {
        public static void SetCurrentAction(this GameTurn turn, GameActionState state, string location)
        {
            if (turn.CurrentAction != null)
            {
                turn.AddCompletedAction(turn.CurrentAction);
            }
            turn.CurrentAction = new GameAction { State = state, Location = location, IsExecutable = true };
        }

        public static void AddPendingAction(this GameTurn turn, GameAction action)
        {
            turn.AddPendingAction(action, PendingPosition.first);
        }

        public static void AddPendingAction(this GameTurn turn, GameAction action, PendingPosition position)
        {
            action.Turn = turn;
            var temp = turn.PendingActions;
            AddPendingOrderHelper(turn, action, position);
            temp.IncrementPendingActionOrder(position);
            temp.Add(action);
            turn.PendingActions = temp;
        }

        public static void AddPendingActions(this GameTurn turn, List<GameActionState> actions, GameActionStatus status, bool isExecutable)
        {
            turn.AddPendingActions(actions, null, status, PendingPosition.first, isExecutable);
        }

        public static void AddPendingActions(this GameTurn turn, List<GameActionState> actions, GameActionStatus status, PendingPosition position, bool isExecutable)
        {
            turn.AddPendingActions(actions, null, status, position, isExecutable);
        }

        public static void AddPendingActions(this GameTurn turn, List<GameActionState> actions, GameAction parent, GameActionStatus status, PendingPosition position, bool isExecutable)
        {
            var temp = turn.PendingActions;
            var newActions = new List<GameAction>();
            foreach (GameActionState state in actions)
            {
                var action = new GameAction { State = state, Status = status, IsExecutable = isExecutable, Parent = parent, Turn = turn };
                AddPendingOrderHelper(turn, action, position);
                newActions.Add(action);
            }
            temp.IncrementPendingActionOrder(position);
            temp.AddRange(newActions);
            turn.PendingActions = temp;
        }

        private static void AddPendingOrderHelper(GameTurn turn, GameAction newAction, PendingPosition position)
        {
            if (position == PendingPosition.first)
            {
                newAction.Order = turn.CurrentActionOrderNumber + 1;
            }
            else
            {
                if (turn.PendingActions.Count > 0)
                    newAction.Order = turn.PendingActions.Select(a => a.Order).OrderByDescending(o => o).FirstOrDefault() + 1;
                else
                    newAction.Order = turn.CurrentActionOrderNumber + 1;
            }
        }
        private static void IncrementPendingActionOrder(this List<GameAction> pendingList, PendingPosition position)
        {
            if (position == PendingPosition.first)
            {
                foreach (GameAction oldAction in pendingList)
                {
                    oldAction.Order += 1;
                }
            }
        }

        public static void RemovePendingAction(this GameTurn turn, GameAction action)
        {
            var temp = turn.PendingActions;
            temp.Remove(action);
            turn.PendingActions = temp;
        }

        public static void RemovePendingAction(this GameTurn turn, GameActionState state)
        {
            var temp = turn.PendingActions;
            var action = temp.FirstOrDefault(a => a.State == state);
            if( action != null)
            {
                temp.Remove(action);
            }
            turn.PendingActions = temp;
        }

        public static void RemoveAllSiblingActions(this GameTurn turn, GameActionState state)
        {
            var temp = turn.PendingActions;
            var action = temp.FirstOrDefault(a => a.State == state);
            if (action != null)
            {
                if (action.Status == GameActionStatus.OptionalExclusive)
                {
                    temp.RemoveAll(a => a.Status == GameActionStatus.OptionalExclusive && a.Order == action.Order);
                }
                else
                {
                    temp.Remove(action);
                }
            }
            turn.PendingActions = temp;
        }

        public static void RemoveAllSiblingActions(this GameTurn turn, GameAction action)
        {
            var temp = turn.PendingActions;
            if (action != null)
            {
                if (action.Status == GameActionStatus.OptionalExclusive)
                {
                    temp.RemoveAll(a => a.Status == GameActionStatus.OptionalExclusive && a.Order == action.Order);
                }
                else
                {
                    temp.Remove(action);
                }
            }
            turn.PendingActions = temp;
        }

        public static List<GameAction> GetNextActions(this GameTurn turn)
        {
            if (turn.PendingActions.Count > 0)
            {
                var orderedActions = turn.PendingActions.OrderBy(a => a.Order);
                int next = orderedActions.First().Order;
                return orderedActions.Where(a => a.Order == next).ToList();
            }
            return null;
        }

        public static void AddCompletedAction(this GameTurn turn, GameAction action)
        {
            if(action.IsComplete == false)
            {
                action.IsComplete = true;
                action.Order = turn.CompletedActions.Count;
                var temp = turn.CompletedActions;
                temp.Add(action);
                turn.CompletedActions = temp;
            }
        }

        public static void SetupFirstTurn(this Game newGame)
        {
            var firstTurn = new GameTurn { TurnNumber = 0, Type = GameTurnType.Setup, CurrentAction = new GameAction { State = GameActionState.GameStart, Order = 0 } };
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
            var newTurn = new GameTurn { Game = game, TurnNumber = oldTurn.TurnNumber + 1, NextActionId = 0, CurrentPlayer = game.CurrentPlayer };
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