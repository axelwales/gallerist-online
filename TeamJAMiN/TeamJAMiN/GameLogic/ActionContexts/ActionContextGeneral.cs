using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameControllerHelpers;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class SetupContext : ActionContext
    {
        private static Dictionary<GameActionState, Type> _nameToState = new Dictionary<GameActionState, Type>
        {
            { GameActionState.TurnStart, typeof(TurnStart) },
            { GameActionState.Pass, typeof(Pass) },
            { GameActionState.GameStart, typeof(GameStart) },
        };

        public SetupContext(Game game) : base(game, _nameToState) { }
        public SetupContext(Game game, GameAction action) : base(game, action, _nameToState) { }
    }

    public class GameStart : ActionState
    {
        public GameStart()
        {
            Name = GameActionState.GameStart;
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }
    }


    public class TurnStart : ActionState
    {
        public TurnStart()
        {
            Name = GameActionState.TurnStart;
            TransitionTo = new HashSet<GameActionState>
            {
                GameActionState.SalesOffice,
                GameActionState.InternationalMarket,
                GameActionState.MediaCenter,
                GameActionState.ArtistColony,
                GameActionState.UseTicket,
                GameActionState.UseContractBonus
            };
        }
    }
    public class Pass : ActionState
    {
        public Pass()
        {
            Name = GameActionState.Pass;
            TransitionTo = new HashSet<GameActionState> { };
        }

        public override void DoAction<ActionContext>(ActionContext context)
        {
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            context.Game.SetupNextTurn();
        }
    }

    public abstract class LocationAction: ActionState
    {
        public PlayerLocation location;
        public override void DoAction<ActionContext>(ActionContext context)
        {
            var game = context.Game;
            if( game.CurrentTurn.Type == GameTurnType.Location)
            {
                if(game.CurrentTurn.HasExecutiveAction)
                {
                    var executiveActions = new List<GameAction>
                    {
                        new GameAction { State = GameActionState.UseContractBonus, Parent = context.Action.Parent, Status = GameActionStatus.Optional, IsExecutable = false },
                        new GameAction { State = GameActionState.UseTicket, Parent = context.Action.Parent, Status = GameActionStatus.Optional, IsExecutable = false },
                    };
                    TurnManager.AddPendingActions(game.CurrentTurn, executiveActions, PendingPosition.first);
                }

                var kickedPlayer = game.Players.FirstOrDefault(p => p.GalleristLocation == location);
                if (kickedPlayer != null)
                {
                    game.CurrentTurn.KickedOutPlayer = kickedPlayer;
                }
                game.CurrentPlayer.GalleristLocation = location;
            }
            if (game.CurrentTurn.Type == GameTurnType.KickedOut)
            {
                //todo remove influence
                game.CurrentTurn.HasExecutiveAction = false;
            }

            base.DoAction(context);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var currentPlayerLocation = game.CurrentPlayer.GalleristLocation;
            if (currentPlayerLocation == location && game.CurrentTurn.Type == GameTurnType.Location)
                return false;

            if (currentPlayerLocation != location && game.CurrentTurn.Type == GameTurnType.KickedOut)
                return false;

            //todo check if child action states are legal
            return true;
        }
    }
}