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
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        public override void DoAction<MediaCenterContext>(MediaCenterContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            var childActions = new List<GameAction>();
            var artist = context.Game.GetArtistByLocationString(context.Action.Location);
            var promotion = ++artist.Promotion;

            var bonusState = BonusManager.BonusTypeToState[artist.DiscoverBonus];
            var bonusExecute = BonusManager.BonusStateIsExecutable[bonusState];
            childActions.Add(new GameAction { State = bonusState, Parent = context.Action, IsExecutable = bonusExecute });

            //todo give player promotion bonus
            var promotionState = BonusManager.PromotionBonusStateByLevel[promotion - 1];
            var promotionExecute = BonusManager.BonusStateIsExecutable[promotionState];
            childActions.Add(new GameAction { State = promotionState, Parent = context.Action, IsExecutable = promotionExecute });

            context.Game.CurrentPlayer.Influence -= promotion;
            artist.Fame += 1;
            artist.Fame += context.Game.CurrentPlayer.GetGalleryVisitorCountByType(VisitorTicketType.collector);

            //todo allow players to increase fame with influence
            childActions.Add(new GameAction { State = GameActionState.UseInfluenceAsFame, Parent = context.Action, IsExecutable = false });

            //todo replace below with a pass button or something.
            TurnManager.AddPendingActions(context.Game.CurrentTurn, childActions, PendingPosition.first);
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            if ( !context.Action.ValidateArtistLocationString() )
            {
                return false;
            }
            var artist = context.Game.GetArtistByLocationString(context.Action.Location);
            if (artist.Promotion == 5)
            {
                return false;
            }
            if( artist.Promotion + 1 > context.Game.CurrentPlayer.Influence)
            {
                return false;
            }
            return true;
        }
    }

    public class Hire : ActionState
    {
        public Hire()
        {
            Name = GameActionState.Hire;
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        public override void DoAction<MediaCenterContext>(MediaCenterContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            var player = context.Game.CurrentPlayer;
            //todo allow players to buy multiple assistants
            player.Money -= player.GetNextAssistantCost();
            player.GetNewAssistant();
            //todo give player hire bonus
            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }
        //todo validate location string
        //todo check is player has room in the office
        //todo check if player has enough money
    }
}