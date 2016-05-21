using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class InternationalMarketContext : ActionContext
    {
        public InternationalMarketContext(Game game)
            : base(game, new Dictionary<GameActionState, Type> {
                { GameActionState.ChooseLocation, typeof(ChooseLocation) },
                { GameActionState.InternationalMarket, typeof(InternationalMarket) },
                { GameActionState.Reputation, typeof(Reputation) },
                { GameActionState.ReputationToBoard, typeof(ReputationToBoard) },
                { GameActionState.Auction, typeof(Auction) },
                { GameActionState.Pass, typeof(Pass) }
            })
        { }
    }

    public class InternationalMarket : LocationAction
    {
        public InternationalMarket()
        {
            Name = GameActionState.InternationalMarket;
            location = PlayerLocation.InternationalMarket;
            TransitionTo = new HashSet<GameActionState> { GameActionState.Reputation, GameActionState.Auction };
        }
    }

    public class Reputation : ActionState
    {
        public Reputation()
        {
            Name = GameActionState.Reputation;
            TransitionTo = new HashSet<GameActionState> { GameActionState.ReputationToBoard };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            game.CurrentTurn.CurrentAction.State = Name;
            var column = context.Game.GetReputationColumn();
            game.CurrentPlayer.Influence += game.GetInfluenceByColumn(column);
            var row = context.Game.GetReputationRow();
            var reputationTile = context.Game.GetReputationTileByLocation(row, column);
            reputationTile.Column = GameReputationTileLocation.ReputationToBoard;
            context.Game.CurrentPlayer.Tiles.Add(reputationTile);
            //todo set assistant location
            base.DoAction(context);
        }
        //todo validate location string
        //todo check if player can take tile (can access that column/row)
        //todo check if player has an assistant
    }
    public class ReputationToBoard : ActionState
    {
        public ReputationToBoard()
        {
            Name = GameActionState.ReputationToBoard;
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            game.CurrentTurn.CurrentAction.State = Name;
            var location = (GameReputationTileLocation)Enum.Parse(typeof(GameReputationTileLocation), context.Action.Location);
            var player = context.Game.CurrentPlayer;
            var reputationTile = player.Tiles.First(c => c.Column == GameReputationTileLocation.ReputationToBoard);
            reputationTile.Column = location;
            //todo give player bonus by location
            //todo send out a visitor from the lobby
            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);

        }
        //todo validate location string
        //todo check if player can take tile (can access that column/row)
        //todo check if player has an assistant
    }

    public class Auction : ActionState
    {
        public Auction()
        {
            Name = GameActionState.Auction;
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            var column = context.Game.GetAuctionColumn();
            game.CurrentPlayer.Influence += game.GetInfluenceByColumn(column);
            var row = context.Game.GetAuctionRow();
            //todo set assistant location
            //give bonus
            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);

        }
        //todo validate location string
        //todo check if player can place an assistant there (can access that column)
        // check if player can produce that much money
        //todo check if player has an assistant
    }
}