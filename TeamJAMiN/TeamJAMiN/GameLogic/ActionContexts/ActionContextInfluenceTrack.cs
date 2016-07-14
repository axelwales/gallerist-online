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
        private static Dictionary<GameActionState, Type> _nameToState = new Dictionary<GameActionState, Type>
        {
            { GameActionState.UseInfluenceAsMoney, typeof(UseInfluenceAsMoney) },
            { GameActionState.UseInfluenceAsFame, typeof(UseInfluenceAsFame) }
        };

        public InfluenceTrackContext(Game game) : base(game, _nameToState) { }
        public InfluenceTrackContext(Game game, GameAction action) : base(game, action, _nameToState) { }
    }

    public class UseInfluenceAsMoney : ActionState
    {
        public UseInfluenceAsMoney()
        {
            Name = GameActionState.UseInfluenceAsMoney;
            RequiredParams = new HashSet<string> { "Location", "Cost" };
            TransitionTo = new HashSet<GameActionState> { };
        }

        public override void DoAction<ActionContext>(ActionContext context)
        {
            var cost = int.Parse(context.Action.StateParams["Cost"]);
            int influenceAsMoney = int.Parse(context.Action.StateParams["Location"]);
            context.Game.CurrentPlayer.UseInfluenceAsMoney(influenceAsMoney);
            cost -= influenceAsMoney;
            context.Game.CurrentPlayer.Money -= cost;
        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            var cost = int.Parse(context.Action.StateParams["Cost"]);

            int influenceAsMoney;
            var locationIsInt = int.TryParse(context.Action.StateParams["Location"], out influenceAsMoney);
            if (locationIsInt == false)
                return false;

            if (context.Game.CurrentPlayer.HasInfluenceAsMoney(influenceAsMoney) == false)
                return false;
            cost -= influenceAsMoney;

            if (context.Game.CurrentPlayer.Money < cost)
                return false;

            return true;
        }
    }

    public class UseInfluenceAsFame : ActionState
    {
        public UseInfluenceAsFame()
        {
            Name = GameActionState.UseInfluenceAsFame;
            RequiredParams = new HashSet<string> { "Location", "Fame" };
            TransitionTo = new HashSet<GameActionState> { };
        }

        public override void DoAction<ActionContext>(ActionContext context)
        {
            int influenceAsFame = int.Parse(context.Action.StateParams["Location"]);
            context.Game.CurrentPlayer.UseInfluenceAsFame(influenceAsFame);

            int baseFameIncrease = int.Parse(context.Action.StateParams["Fame"]);
            var artist = context.Game.GetArtistByLocationString(context.Action.Parent.StateParams["Location"]);
            artist.Fame += influenceAsFame + baseFameIncrease;
        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            int influenceAsFame;
            var locationIsInt = int.TryParse(context.Action.StateParams["Location"], out influenceAsFame);
            if (locationIsInt == false)
                return false;

            if (context.Game.CurrentPlayer.HasInfluenceAsFame(influenceAsFame) == false)
                return false;

            return true;
        }
    }
}