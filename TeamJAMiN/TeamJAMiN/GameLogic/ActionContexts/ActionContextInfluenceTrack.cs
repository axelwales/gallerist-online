using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.GameLogic.ActionContexts
{
    public class InfluenceTrackContext : NonLinearActionContext
    {
        public InfluenceTrackContext(Game game)
            : base(game, new Dictionary<GameActionState, Type>
            {
                { GameActionState.UseInfluenceAsMoney, typeof(UseInfluenceAsMoney) },
                { GameActionState.UseInfluenceAsFame, typeof(UseInfluenceAsFame) }
            })
        { }
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
            if (parentContext is IMoneyTransactionContext == false)
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
            if (context.Game.CurrentPlayer.HasInfluenceAsMoney(influenceAsMoney) == false)
            {
                return false;
            }
            cost -= influenceAsMoney;
            if (context.Game.CurrentPlayer.Money < cost)
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