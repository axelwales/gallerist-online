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
                { GameActionState.UseInfluenceAsMoney, typeof(UseInfluenceAsMoney) },
                { GameActionState.UseInfluenceAsFame, typeof(UseInfluenceAsFame) }
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

    public interface IMoneyTransactionState
    {
        int GetCost(ActionContext context);
        void CleanUp(ActionContext context);
    }

    public interface IMoneyTransactionContext
    {
        int GetCost();
        bool IsMoneyTransaction();
        void CleanUp();
    }

    public class UseInfluenceAsMoney : ActionState
    {
        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            var parentState = context.Action.Parent.State;
            var parentContext = (IMoneyTransactionContext)ActionContextFactory.GetContext(parentState, context.Game);
            var cost = parentContext.GetCost();
            int influenceAsMoney = int.Parse(context.Action.Location);
            context.Game.CurrentPlayer.UseInfluenceAsMoney(influenceAsMoney);
            cost -= influenceAsMoney;
            context.Game.CurrentPlayer.Money -= cost;
            parentContext.CleanUp();
        }

        public override bool IsValidGameState(ActionContext context)
        {
            var parentState = context.Action.Parent.State;
            var parentContext = ActionContextFactory.GetContext(parentState, context.Game);
            if (parentContext is IMoneyTransactionContext == false )
            {
                return false;
            }
            var transactionContext = (IMoneyTransactionContext)parentContext;
            if (transactionContext.IsMoneyTransaction() == false)
            {
                return false;
            }
            var cost = transactionContext.GetCost();
            int influenceAsMoney;
            var locationIsInt = int.TryParse(context.Action.Location, out influenceAsMoney);
            if (locationIsInt == false)
            {
                return false;
            }
            if(context.Game.CurrentPlayer.HasInfluenceAsMoney(influenceAsMoney) == false)
            {
                return false;
            }
            cost -= influenceAsMoney;
            if(context.Game.CurrentPlayer.Money < cost)
            {
                return false;
            }
            return true;
        }
    }

    public class UseInfluenceAsFame : ActionState
    {
        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            int influenceAsFame = int.Parse(context.Action.Location);
            context.Game.CurrentPlayer.UseInfluenceAsFame(influenceAsFame);
            var artist = context.Game.GetArtistByLocationString(context.Action.Parent.Location);
            artist.Fame += influenceAsFame;
        }

        public override bool IsValidGameState(ActionContext context)
        {
            int influenceAsFame;
            var locationIsInt = int.TryParse(context.Action.Location, out influenceAsFame);
            if (locationIsInt == false)
            {
                return false;
            }
            if (context.Game.CurrentPlayer.HasInfluenceAsFame(influenceAsFame) == false)
            {
                return false;
            }
            return true;
        }
    }

}