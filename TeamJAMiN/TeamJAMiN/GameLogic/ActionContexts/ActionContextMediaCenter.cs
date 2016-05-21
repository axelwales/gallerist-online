using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class MediaCenterContext : ActionContext
    {
        public MediaCenterContext(Game game)
            : base(game, new Dictionary<GameActionState, Type> {
                { GameActionState.ChooseLocation, typeof(ChooseLocation) },
                { GameActionState.MediaCenter, typeof(MediaCenter) },
                { GameActionState.Promote, typeof(Promote) },
                { GameActionState.Hire, typeof(Hire) },
                { GameActionState.Pass, typeof(Pass) }
            })
        { }
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
            var artist = context.Game.GetArtistByLocationString(context.Action.Location);
            var promotion = ++artist.Promotion;
            //todo give player promotion bonus
            context.Game.CurrentPlayer.Influence -= promotion;
            artist.Fame += 1;
            artist.Fame += context.Game.CurrentPlayer.GetGalleryVisitorCountByType(VisitorTicketType.collector);
            //todo allow players to increase fame with influence
            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }
        //todo validate location string
        //todo check is artist can be promoted
        //todo check if player has enough influence
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
        //todo check is artist can be promoted
        //todo check if player has enough influence
    }
}