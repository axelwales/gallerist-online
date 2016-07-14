using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.GameLogic.ActionContexts;
using TeamJAMiN.GameLogic.ComponentManagers;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class ArtistColonyContext : ActionContext
    {
        private static Dictionary<GameActionState, Type> _nameToState = new Dictionary<GameActionState, Type>
        {
            { GameActionState.ChooseLocation, typeof(ChooseLocation) },
            { GameActionState.ArtistColony, typeof(ArtistColony) },
            { GameActionState.ArtistDiscover, typeof(ArtistDiscover) },
            { GameActionState.ArtBuy, typeof(ArtBuy) },
            { GameActionState.Pass, typeof(Pass) }
        };

        public ArtistColonyContext(Game game) : base(game, _nameToState) { }
        public ArtistColonyContext(Game game, GameAction action) : base(game, action, _nameToState) { }
    }

    public class ArtistColony : LocationAction
    {
        public ArtistColony()
        {
            Name = GameActionState.ArtistColony;
            location = PlayerLocation.ArtistColony;
            TransitionTo = new HashSet<GameActionState> { GameActionState.ArtistDiscover, GameActionState.ArtBuy };
        }
    }

    public class ArtBuy : ActionState
    {
        public ArtBuy()
        {
            Name = GameActionState.ArtBuy;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> {  GameActionState.Pass };
        }

        public int GetCost(GameArtist artist, Game game)
        {
            if(game.CurrentPlayer.Commission == artist)
            {
                return artist.InitialFame;
            }
            return artist.Fame;
        }

        public int GetFame(GameArt art, Game game)
        {
            return art.Fame + game.CurrentPlayer.GetGalleryVisitorCountByType(VisitorTicketType.collector);
        }

        public override void DoAction<ArtistColonyContext>(ArtistColonyContext context)
        {
            var game = context.Game;
            var turn = game.CurrentTurn;
            var childActions = new List<GameAction>();
            var artist = context.Game.GetArtistByLocationString(context.Action.StateParams["Location"]);
            var art = context.Game.GetArtFromStack(artist.ArtType);
            art.Artist = artist;
            context.Game.MoveFromArtStackToPlaza(art.Type);
            if (context.Game.CurrentPlayer.Commission != artist)
            {
                artist.AvailableArt -= 1;
            }

            var moneyAction = new GameAction { State = GameActionState.UseInfluenceAsMoney, Parent = context.Action, IsExecutable = false };
            moneyAction.StateParams.Add( "Cost", GetCost(artist, game).ToString() );
            childActions.Add(moneyAction);

            var fameAction = new GameAction { State = GameActionState.UseInfluenceAsFame, Parent = context.Action, IsExecutable = false };
            fameAction.StateParams.Add("Fame", GetFame(art, game).ToString() );
            childActions.Add(fameAction);
            context.Game.CurrentPlayer.Art.Add(art);

            var ticketStates = art.GetArtTicketActionStates();
            foreach( GameActionState ticketState in ticketStates)
            {
                bool isExecutable;
                if (ticketState == GameActionState.GetTicketVip || ticketState == GameActionState.GetTicketCollector || ticketState == GameActionState.GetTicketInvestor)
                {
                    isExecutable = true;
                }
                else
                    isExecutable = false;
                childActions.Add(new GameAction { State = ticketState, Parent = context.Action, IsExecutable = isExecutable });
            }

            if (context.Game.CurrentPlayer.Commission == artist)
                context.Game.CurrentPlayer.Commission = null;

            //todo see if player should gain reputation tile
            context.Game.SetupNextArt(artist.ArtType);
            //todo replace below with a pass button or something.
            TurnManager.AddPendingActions(turn, childActions, PendingPosition.first);
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }
        //check if artist is a celebrity
        //todo check if player has room to exhibit art
        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            if (!context.Action.ValidateArtistLocationString())
                return false;

            var artist = context.Game.GetArtistByLocationString(context.Action.StateParams["Location"]);
            if (!artist.IsDiscovered)
                return false;

            if (artist.AvailableArt == 0)
                return false;

            if (context.Game.CurrentPlayer.Art.Where(a => a.IsSold == false).Count() > 3)
                return false;

            return true;
        }
    }

    public class ArtistDiscover : ActionState
    {
        public ArtistDiscover()
        {
            Name = GameActionState.ArtistDiscover;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        public override void DoAction<ArtistColonyContext>(ArtistColonyContext context)
        {
            var game = context.Game;
            var artist = context.Game.GetArtistByLocationString(context.Action.StateParams["Location"]);
            artist.IsDiscovered = true;

            if(artist.Category == ArtistCategory.red)
            {
                var newCollector = new GameVisitor { Location = GameVisitorLocation.Plaza, Type = VisitorTicketType.collector };
                game.Visitors.Add(newCollector);
            }

            context.Game.CurrentPlayer.Commission = artist;
            artist.AvailableArt -= 1;

            var bonusState = BonusManager.BonusTypeToState[artist.DiscoverBonus];
            var isExecutable = BonusManager.BonusStateIsExecutable[bonusState];
            context.Game.CurrentTurn.AddPendingAction(new GameAction { State = bonusState, Parent = context.Action, IsExecutable = isExecutable });

            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }
        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            if (!context.Action.ValidateArtistLocationString())
                return false;

            var artist = context.Game.GetArtistByLocationString(context.Action.StateParams["Location"]);
            if (artist.IsDiscovered)
                return false;

            if (context.Game.CurrentPlayer.Commission != null)
                return false;

            return true;
        }
    }
}