using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class ReputationManager
    {
        public static Dictionary<GameReputationTileLocation, int> InfluenceByColumn = new Dictionary<GameReputationTileLocation, int>()
        {
            { GameReputationTileLocation.OneInfluence, 1 },
            { GameReputationTileLocation.ThreeInfluence, 3 },
            { GameReputationTileLocation.TwoInfluence, 2 }
        };

        public static Dictionary<GameReputationTileLocation, GameActionState> TileLocationToBonus = new Dictionary<GameReputationTileLocation, GameActionState>
        {
            { GameReputationTileLocation.Assistant, GameActionState.GetAssistant },
            { GameReputationTileLocation.Fame, GameActionState.ChooseArtistFame },
            { GameReputationTileLocation.Influence, GameActionState.GetInfluence },
            { GameReputationTileLocation.Money, GameActionState.GetMoney },
            { GameReputationTileLocation.Tickets, GameActionState.ChooseTicketAnyTwo },
            { GameReputationTileLocation.Visitor, GameActionState.ChooseVisitorFromPlaza }
        };

        public static Dictionary<string, int> PositionByColumn = new Dictionary<string, int>()
        {
            { "OneInfluence", 3 },
            { "ThreeInfluence", 1 },
            { "TwoInfluence", 2 }
        };

        public static Dictionary<string, int> PositionByRow = new Dictionary<string, int>()
        {
            { "digital", 0 },
            { "photo", 1 },
            { "sculpture", 2 },
            { "painting", 3 }
        };

        public static GameReputationTileLocation GetReputationColumn(string location)
        {
            var locationParams = location.Split(':');
            return (GameReputationTileLocation)Enum.Parse(typeof(GameReputationTileLocation),locationParams[1]);
        }

        public static ArtType GetReputationRow(string location)
        {
            var locationParams = location.Split(':');
            return (ArtType)Enum.Parse(typeof(ArtType), locationParams[0]);
        }

        public static GameReputationTile GetReputationTileByLocation(this Game game, ArtType row, GameReputationTileLocation column)
        {
            return game.ReputationTiles.Where(r => r.Column == column && r.Row == row).Single();
        }

        public static int GetInfluenceByColumn(this Game game, GameReputationTileLocation column)
        {
            if(InfluenceByColumn.ContainsKey(column))
            {
                return InfluenceByColumn[column];
            }
            return 0;
        }

        public static bool IsLegalColumn(this Player player, GameReputationTileLocation column)
        {
            var collectors = player.GetLobbyVisitorCountByType(VisitorTicketType.collector);
            var investors = player.GetLobbyVisitorCountByType(VisitorTicketType.investor);
            var vips = player.GetLobbyVisitorCountByType(VisitorTicketType.vip);

            switch (column)
            {
                case GameReputationTileLocation.ThreeInfluence:
                    if (collectors == 0 || (investors == 0 && vips == 0))
                        return false;
                    break;
                case GameReputationTileLocation.TwoInfluence:
                    if (investors == 0 && vips == 0)
                        return false;
                    break;
                case GameReputationTileLocation.OneInfluence:
                    if (collectors == 0 && vips == 0 && investors == 0)
                        return false;
                    break;
                default:
                    return false;
            }

            return true;
        }

        public static PlayerAssistantLocation GetAuctionLocationByLocationString(string currentLocation)
        {
            var row = GetReputationRow(currentLocation);
            var column = GetReputationColumn(currentLocation);
            var reputationIndex = PositionByColumn[column.ToString()] + 3 * PositionByRow[row.ToString()];
            var location = (PlayerAssistantLocation)Enum.Parse(typeof(PlayerAssistantLocation), "Reputation" + reputationIndex);
            return location;
        }
    }
}