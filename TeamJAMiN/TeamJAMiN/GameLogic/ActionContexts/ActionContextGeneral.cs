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
        public SetupContext(Game game)
            : base(game, new Dictionary<GameActionState, Type>
            {
                { GameActionState.ChooseLocation, typeof(ChooseLocation) },
                { GameActionState.Pass, typeof(Pass) },
                { GameActionState.GameStart, typeof(GameStart) },
            })
        { }

    }

    public class GameStart : ActionState
    {
        public GameStart()
        {
            Name = GameActionState.GameStart;
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }
    }


    public class ChooseLocation : ActionState
    {
        public ChooseLocation()
        {
            Name = GameActionState.ChooseLocation;
            TransitionTo = new HashSet<GameActionState>
                { GameActionState.SalesOffice, GameActionState.InternationalMarket, GameActionState.MediaCenter, GameActionState.ArtistColony };
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
        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            var game = context.Game;
            var kickedPlayer = game.Players.FirstOrDefault(p => p.GalleristLocation == location);
            if (kickedPlayer != null)
            {
                game.KickedOutPlayerId = kickedPlayer.Id;
            }
            game.CurrentPlayer.GalleristLocation = location;
            base.DoAction(context);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var currentPlayerLocation = game.CurrentPlayer.GalleristLocation;
            if (currentPlayerLocation == (PlayerLocation)Enum.Parse(typeof(PlayerLocation), Name.ToString()))
            {
                return false;
            }
            return true;
        }
    }
}