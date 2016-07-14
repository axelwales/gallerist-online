using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class AuctionManager
    {
        public static Dictionary<string, int> InfluenceByColumn = new Dictionary<string, int>()
        {
            { "OneInfluence", 1 },
            { "ThreeInfluence", 3 },
            { "TwoInfluence", 2 }
        };

        public static Dictionary<string, int> PositionByColumn = new Dictionary<string, int>()
        {
            { "OneInfluence", 3 },
            { "ThreeInfluence", 1 },
            { "TwoInfluence", 2 }
        };

        public static Dictionary<string, int> CostByRow = new Dictionary<string, int>()
        {
            { "Auction1", 1 },
            { "Auction3", 2 },
            { "Auction6", 6 }
        };

        public static Dictionary<string, int> PositionByRow = new Dictionary<string, int>()
        {
            { "Auction1", 0 },
            { "Auction3", 1 },
            { "Auction6", 2 }
        };

        public static Dictionary<PlayerAssistantLocation, BonusType> LocationToBonus = new Dictionary<PlayerAssistantLocation, BonusType>()
        {
            { PlayerAssistantLocation.Auction1, BonusType.ticket },
            { PlayerAssistantLocation.Auction2, BonusType.assistant },
            { PlayerAssistantLocation.Auction3, BonusType.twoTickets },
            { PlayerAssistantLocation.Auction4, BonusType.assistant },
            { PlayerAssistantLocation.Auction5, BonusType.influence },
            { PlayerAssistantLocation.Auction6, BonusType.money },
            { PlayerAssistantLocation.Auction7, BonusType.plazaVisitor },
            { PlayerAssistantLocation.Auction8, BonusType.money },
            { PlayerAssistantLocation.Auction9, BonusType.influence }
        };


        public static string GetAuctionColumn(this Game game)
        {
            var currentLocation = game.CurrentTurn.CurrentAction.StateParams["Location"];
            return GetAuctionColumnByLocationString(currentLocation);
        }

        public static string GetAuctionColumnByLocationString(string currentLocation)
        {
            var locationParams = currentLocation.Split(':');
            return locationParams[1];
        }

        public static string GetAuctionRow(this Game game)
        {
            var currentLocation = game.CurrentTurn.CurrentAction.StateParams["Location"];
            return GetAuctionRowByLocationString(currentLocation);
        }

        public static string GetAuctionRowByLocationString(string currentLocation)
        {
            var locationParams = currentLocation.Split(':');
            return locationParams[0];
        }

        public static int GetInfluenceByColumn(this Game game, string column)
        {
            if (InfluenceByColumn.ContainsKey(column))
            {
                return InfluenceByColumn[column];
            }
            return 0;
        }

        public static PlayerAssistantLocation GetAuctionLocation(this Game game)
        {
            var location = game.CurrentTurn.CurrentAction.StateParams["Location"];
            return GetAuctionLocationByLocationString(location);
        }

        public static BonusType GetAuctionBonus(this Game game)
        {
            var location = game.CurrentTurn.CurrentAction.StateParams["Location"];
            return GetBonusByAuctionLocationString(location);
        }

        public static PlayerAssistantLocation GetAuctionLocationByLocationString(string currentLocation)
        {
            var row = GetAuctionRowByLocationString(currentLocation);
            var column = GetAuctionColumnByLocationString(currentLocation);
            var auctionIndex = PositionByColumn[column] + 3 * PositionByRow[row];
            var location = (PlayerAssistantLocation)Enum.Parse(typeof(PlayerAssistantLocation), "Auction" + auctionIndex);
            return location;
        }

        public static BonusType GetBonusByAuctionLocationString(string location)
        {
            var assistantLocation = GetAuctionLocationByLocationString(location);
            return LocationToBonus[assistantLocation];
        }
    }
}