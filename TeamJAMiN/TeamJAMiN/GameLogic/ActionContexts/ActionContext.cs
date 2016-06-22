using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class ActionContext
    {
        protected ActionState _state;
        public Dictionary<GameActionState, Type> NameToState;
        public Game Game { get; set; }
        public GameAction Action { get; set; }

        public GameActionState State
        {
            get
            {
                return GetState();
            }
            set
            {
                SetState(value);
            }
        }

        protected virtual GameActionState GetState()
        {
            return _state.Name;
        }

        protected virtual void SetState(GameActionState state)
        {
            _state = (ActionState)Activator.CreateInstance(NameToState[state]);
        }

        protected ActionContext(Game game, Dictionary<GameActionState, Type> NameToState)
        {
            Game = game;
            Action = game.CurrentTurn.CurrentAction;
            this.NameToState = NameToState;
            State = Game.CurrentTurn.CurrentAction.State;
        }

        protected ActionContext(Game game, GameAction action, Dictionary<GameActionState, Type> NameToState)
        {
            Game = game;
            Action = action;
            this.NameToState = NameToState;
            State = action.State;
        }

        public bool IsValidGameState(GameAction action)
        {
            State = action.State;
            Action = action;
            return _state.IsValidGameState(this);
        }

        public virtual void DoAction(GameActionState state)
        {
            var action = new GameAction { State = state, IsExecutable = true, Location = "" };
            DoAction(action);
        }

        public virtual void DoAction(GameAction action)
        {
            Action = action;
            State = action.State;
            _state.DoAction(this);
        }

    }

    public abstract class ActionState
    {
        public GameActionState Name;
        public HashSet<GameActionState> TransitionTo;
        public virtual void DoAction<TContext>(TContext context)
            where TContext : ActionContext
        {
            if(TransitionTo.Count <= 1 )
            {
                context.Game.CurrentTurn.AddPendingActions(TransitionTo.ToList(), GameActionStatus.Optional, false);
            }
            else
            {
                context.Game.CurrentTurn.AddPendingActions(TransitionTo.ToList(), context.Action, GameActionStatus.OptionalExclusive, PendingPosition.first, false);
            }
        }
        public virtual bool IsValidGameState(ActionContext context)
        {
            return true;
        }

        public void AddPassAction(ActionContext context)
        {
            context.Game.CurrentTurn.AddPendingAction(new GameAction { Parent = context.Action, State = GameActionState.Pass, IsExecutable = true }, PendingPosition.last);
        }

    }

    public abstract class NonLinearActionContext : ActionContext
    {
        protected NonLinearActionContext(Game game, Dictionary<GameActionState, Type> NameToState) : base(game, NameToState)
        {

        }

        public bool IsParentState { get; private set; }
        public ActionContext ParentContext { get; private set; }

        protected override GameActionState GetState()
        {
            if (!IsParentState)
                return base.GetState();
            else
                return ParentContext.State;
        }

        protected override void SetState(GameActionState state)
        {
            if (NameToState.ContainsKey(state))
            {
                IsParentState = false;
                base.SetState(state);
            }
            else
            {
                IsParentState = true;
                ParentContext = ActionContextFactory.GetContext(state, Game);
            }
        }

        public override void DoAction(GameAction action)
        {
            if (!IsParentState)
                base.DoAction(action);
        }
    }
}