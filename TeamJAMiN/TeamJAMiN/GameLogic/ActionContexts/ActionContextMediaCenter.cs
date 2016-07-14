using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.GameLogic.ComponentManagers;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class MediaCenterContext : ActionContext
    {
        private static Dictionary<GameActionState, Type> _nameToState = new Dictionary<GameActionState, Type>
        {
            { GameActionState.ChooseLocation, typeof(ChooseLocation) },
            { GameActionState.MediaCenter, typeof(MediaCenter) },
            { GameActionState.Promote, typeof(Promote) },
            { GameActionState.Hire, typeof(Hire) },
            { GameActionState.Pass, typeof(Pass) }
        };

        public MediaCenterContext(Game game) : base(game, _nameToState) { }
        public MediaCenterContext(Game game, GameAction action) : base(game, action, _nameToState) { }

    }

    public class MediaCenter : LocationAction
    {
        public MediaCenter()
        {
            Name = GameActionState.MediaCenter;
            location = PlayerLocation.MediaCenter;
            TransitionTo = new HashSet<GameActionState> { GameActionState.Promote, GameActionState.Hire };
        }
    }

    public class Promote : ActionState
    {
        public Promote()
        {
            Name = GameActionState.Promote;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        public override void DoAction<MediaCenterContext>(MediaCenterContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            var childActions = new List<GameAction>();
            var artist = context.Game.GetArtistByLocationString(context.Action.StateParams["Location"]);
            var promotion = ++artist.Promotion;

            //todo give player promotion bonus
            var promotionState = BonusManager.PromotionBonusStateByLevel[promotion - 1];
            var promotionExecute = BonusManager.BonusStateIsExecutable[promotionState];
            childActions.Add(new GameAction { State = promotionState, Parent = context.Action, IsExecutable = promotionExecute });

            context.Game.CurrentPlayer.Influence -= promotion;

            var fame = 1 + context.Game.CurrentPlayer.GetGalleryVisitorCountByType(VisitorTicketType.collector);
            var fameAction = new GameAction { State = GameActionState.UseInfluenceAsFame, Parent = context.Action, IsExecutable = false };
            fameAction.StateParams.Add("Fame", fame.ToString());
            childActions.Add(fameAction);

            //todo replace below with a pass button or something.
            TurnManager.AddPendingActions(context.Game.CurrentTurn, childActions, PendingPosition.first);
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            if ( !context.Action.ValidateArtistLocationString() )
                return false;

            var artist = context.Game.GetArtistByLocationString(context.Action.StateParams["Location"]);
            if (artist.Promotion == 5)
                return false;

            if( artist.Promotion + 1 > context.Game.CurrentPlayer.Influence)
                return false;

            return true;
        }
    }

    public class Hire : ActionState
    {
        public Hire()
        {
            Name = GameActionState.Hire;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        private int GetCost(int startIndex, int endIndex)
        {
            int cost = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                cost += AssistantManager.AssistantCost[i];
            }
            return cost;
        }

        public override void DoAction<MediaCenterContext>(MediaCenterContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            var player = context.Game.CurrentPlayer;
            var childActions = new List<GameAction>();

            var index = int.Parse(context.Action.StateParams["Location"]);
            int currentIndex = context.Game.CurrentPlayer.Assistants.Count - 2;
            int cost = GetCost(currentIndex, index);
            player.Money -= cost;
            for (int i = currentIndex; i <= index; i++)
                childActions.Add(new GameAction
                {
                    State = GameActionState.GetAssistant,
                    IsExecutable = true,
                    Parent = context.Action,
                });

            TurnManager.AddPendingActions(context.Game.CurrentTurn, childActions, PendingPosition.first);
            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            int index;
            if (!int.TryParse(context.Action.StateParams["Location"], out index))
                return false;
            int currentIndex = context.Game.CurrentPlayer.Assistants.Count - 2;
            if (index < currentIndex)
                return false;
            int availableDesks = context.Game.CurrentPlayer.Assistants.Where(a => a.Location == PlayerAssistantLocation.Office).Count();
            int requiredDesks = index - currentIndex + 1;
            if (requiredDesks > availableDesks)
                return false;
            int cost = GetCost(currentIndex, index);
            int maxMoney = context.Game.CurrentPlayer.GetMaxMoneyFromInfluence();
            if (cost > maxMoney)
            {
                return false;
            }

            return true;
        }
    }
}