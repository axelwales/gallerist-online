using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.GameLogic.ComponentManagers;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class InternationalMarketContext : ActionContext
    {
        private static Dictionary<GameActionState, Type> _nameToState = new Dictionary<GameActionState, Type>
        {
            { GameActionState.TurnStart, typeof(TurnStart) },
            { GameActionState.InternationalMarket, typeof(InternationalMarket) },
            { GameActionState.Reputation, typeof(Reputation) },
            { GameActionState.ReputationToBoard, typeof(ReputationToBoard) },
            { GameActionState.MoveVisitorFromLobby, typeof(MoveVisitorFromLobby) },
            { GameActionState.Auction, typeof(Auction) },
            { GameActionState.Pass, typeof(Pass) }
        };

        public InternationalMarketContext(Game game) : base(game, _nameToState) { }
        public InternationalMarketContext(Game game, GameAction action) : base(game, action, _nameToState) { }
    }

    public class InternationalMarket : LocationAction
    {
        public InternationalMarket()
        {
            Name = GameActionState.InternationalMarket;
            location = PlayerLocation.InternationalMarket;
            TransitionTo = new HashSet<GameActionState> { GameActionState.Reputation, GameActionState.Auction };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            base.DoAction<InternationalMarketContext>(context);
            //todo check if play must choose an assistant from a location action space
        }
    }

    public class Reputation : ActionState
    {
        public Reputation()
        {
            Name = GameActionState.Reputation;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.ReputationToBoard };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            game.CurrentTurn.CurrentAction.State = Name;

            var location = context.Action.StateParams["Location"];

            var column = ReputationManager.GetReputationColumn(location);
            game.CurrentPlayer.Influence += game.GetInfluenceByColumn(column);

            var row = ReputationManager.GetReputationRow(location);
            var reputationTile = context.Game.GetReputationTileByLocation(row, column);
            reputationTile.Column = GameReputationTileLocation.ReputationToBoard;
            context.Game.CurrentPlayer.Tiles.Add(reputationTile);

            //todo allow player to choose which assistant gets place in case one needs to be taken from a location (this should be done when location is chosen)
            var assistant = game.CurrentPlayer.Assistants.First(a => a.Location == PlayerAssistantLocation.Office);
            assistant.Location = AssistantManager.GetAssistantLocationFromIMLocation(row.ToString(), column.ToString());

            base.DoAction(context);
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);

        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (base.IsValidGameState(context) == false )
                return false;

            var tileCount = context.Game.CurrentPlayer.Tiles.Count;
            if (context.Game.CurrentPlayer.Tiles.Any(t => t.Column == GameReputationTileLocation.Exhibit))
                tileCount -= 1;
            if (tileCount == 6)
                return false;

            var location = context.Action.StateParams["Location"];
            var locationParams = location.Split(':');
            if (locationParams.Count() != 2)
                return false;

            if (context.Game.CurrentPlayer.HasAvailableAssistant() == false)
                return false;

            ArtType row;
            if (Enum.TryParse(locationParams[0], out row) == false)
                return false;

            GameReputationTileLocation column;
            if (Enum.TryParse(locationParams[1], out column) == false)
                return false;

            if (context.Game.CurrentPlayer.Art.Any(a => a.Type == row) == false)
                return false;

            if (ReputationManager.IsLegalColumn(context.Game.CurrentPlayer, column) == false)
                return false;

            return true;
        }
    }

    public class ReputationToBoard : ActionState
    {
        public ReputationToBoard()
        {
            Name = GameActionState.ReputationToBoard;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.MoveVisitorFromLobby };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            game.CurrentTurn.CurrentAction.State = Name;

            var childActions = new List<GameAction>();
            var location = (GameReputationTileLocation)Enum.Parse(typeof(GameReputationTileLocation), context.Action.StateParams["Location"]);
            var player = context.Game.CurrentPlayer;
            var reputationTile = player.Tiles.First(c => c.Column == GameReputationTileLocation.ReputationToBoard);
            reputationTile.Column = location;

            var bonusState = ReputationManager.TileLocationToBonus[location];
            var bonusExecutable = BonusManager.BonusStateIsExecutable[bonusState];
            childActions.Add(new GameAction { State = bonusState, Parent = context.Action, IsExecutable = bonusExecutable });

            if ( VisitorManager.GetVisitorCountAtLocation(game, player.Color, GameVisitorLocation.Lobby) != 0 )
                childActions.Add(new GameAction { State = GameActionState.MoveVisitorFromLobby, Parent = context.Action, IsExecutable = false });

            TurnManager.AddPendingActions(context.Game.CurrentTurn, childActions, PendingPosition.first);
            //todo replace below with a pass button or something.

        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            GameReputationTileLocation location;
            if (!Enum.TryParse(context.Action.StateParams["Location"], out location))
                return false;

            if (context.Game.ReputationTiles.Any(t => t.Column == location))
                return false;

            return true;
        }
    }

    public class MoveVisitorFromLobby : ActionState
    {
        public MoveVisitorFromLobby()
        {
            Name = GameActionState.MoveVisitorFromLobby;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            game.CurrentTurn.CurrentAction.State = Name;

            var type = VisitorManager.GetVisitorTypeFromLocationString(context.Action.StateParams["Location"]);
            var player = context.Game.CurrentPlayer;
            player.MoveVisitor(type, GameVisitorLocation.Lobby, GameVisitorLocation.Plaza);

            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
        }
        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;
            if (!VisitorManager.ValidateVisitorLocationString(context.Action.StateParams["Location"]))
                return false;

            var type = VisitorManager.GetVisitorTypeFromLocationString(context.Action.StateParams["Location"]);
            var player = context.Game.CurrentPlayer;
            var count = VisitorManager.GetLobbyVisitorCountByType(player, type);
            if (count == 0)
            {
                return false;
            }

            return true;
        }

    }

    public class Auction : ActionState
    {
        public Auction()
        {
            Name = GameActionState.Auction;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            //todo move setting current action state to wrapper method
            var game = context.Game;
            var childActions = new List<GameAction>();

            var column = context.Game.GetAuctionColumn();
            game.CurrentPlayer.Influence += game.GetInfluenceByColumn(column);

            var row = context.Game.GetAuctionRow();
            var cost = AuctionManager.CostByRow[row];
            var payAction = new GameAction { State = GameActionState.UseInfluenceAsMoney, Parent = context.Action, IsExecutable = false };
            payAction.StateParams["Cost"] = cost.ToString();
            childActions.Add(payAction);

            //todo set assistant location
            var assistant = game.CurrentPlayer.Assistants.First(a => a.Location == PlayerAssistantLocation.Office);
            assistant.Location = AuctionManager.GetAuctionLocationByLocationString(context.Action.StateParams["Location"]);

            //give bonus
            var bonus = AuctionManager.LocationToBonus[assistant.Location];
            var bonusState = BonusManager.BonusTypeToState[bonus];
            var bonusExecutable = BonusManager.BonusStateIsExecutable[bonusState];
            var bonusAction = new GameAction { State = bonusState, IsExecutable = bonusExecutable, Parent = context.Action };
            childActions.Add(bonusAction);
            //todo replace below with a pass button or something.
            TurnManager.AddPendingActions(context.Game.CurrentTurn, childActions, PendingPosition.first);
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);

        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (base.IsValidGameState(context) == false)
                return false;

            var location = context.Action.StateParams["Location"];
            var locationParams = location.Split(':');
            if (locationParams.Count() != 2)
                return false;

            if (context.Game.CurrentPlayer.HasAvailableAssistant() == false)
                return false;

            if (locationParams[0] != "Auction1" && locationParams[0] != "Auction3" && locationParams[0] != "Auction6")
                return false;

            var cost = AuctionManager.CostByRow[locationParams[0]];
            var maxMoney = InfluenceManager.GetMaxMoneyFromInfluence(context.Game.CurrentPlayer) + context.Game.CurrentPlayer.Money;
            if (cost > maxMoney)
                return false;

            GameReputationTileLocation column;
            if (Enum.TryParse(locationParams[1], out column) == false)
                return false;

            if (ReputationManager.IsLegalColumn(context.Game.CurrentPlayer, column) == false)
                return false;

            return true;
        }
    }
}