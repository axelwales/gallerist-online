using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class AssistantManager
    {
        public static int[] AssistantCost = new int[] { 1, 2, 2, 3, 3, 4, 5, 6 };
        public static GameActionState[] AssistantBonus = new GameActionState[] 
        {
            GameActionState.NoAction,
            GameActionState.GetTicketInvestor,
            GameActionState.GetTicketVip,
            GameActionState.GetInfluence,
            GameActionState.NoAction,
            GameActionState.ChooseTicketAny,
            GameActionState.NoAction,
            GameActionState.GetMoney
        };

        public static void GetNewAssistant(this Player player)
        {
            var assistant = new PlayerAssistant() { Location = PlayerAssistantLocation.Office };
            player.Assistants.Add(assistant);
            player.Game.Assistants.Add(assistant);
        }
        public static int GetNextAssistantCost(this Player player)
        {
            int costIndex = player.Assistants.Count - 2;
            return AssistantCost[costIndex];
        }

        public static bool HasAvailableAssistant(this Player player)
        {
            return player
                .Assistants
                .Any
                (a =>
                    a.Location == PlayerAssistantLocation.Office ||
                    a.Location == PlayerAssistantLocation.ArtistColony ||
                    a.Location == PlayerAssistantLocation.MediaCenter ||
                    a.Location == PlayerAssistantLocation.InternationalMarket ||
                    a.Location == PlayerAssistantLocation.SalesOffice
                );
        }
        public static Dictionary<string, PlayerAssistantLocation> IMLocationToAssistantLocation = new Dictionary<string, PlayerAssistantLocation>
        {
            { "digital:OneInfluence", PlayerAssistantLocation.Reputation3 },
            { "digital:TwoInfluence", PlayerAssistantLocation.Reputation2 },
            { "digital:ThreeInfluence", PlayerAssistantLocation.Reputation1 },
            { "photo:OneInfluence", PlayerAssistantLocation.Reputation6 },
            { "photo:TwoInfluence", PlayerAssistantLocation.Reputation5 },
            { "photo:ThreeInfluence", PlayerAssistantLocation.Reputation4 },
            { "sculpture:OneInfluence", PlayerAssistantLocation.Reputation9 },
            { "sculpture:TwoInfluence", PlayerAssistantLocation.Reputation8 },
            { "sculpture:ThreeInfluence", PlayerAssistantLocation.Reputation7 },
            { "painting:OneInfluence", PlayerAssistantLocation.Reputation12 },
            { "painting:TwoInfluence", PlayerAssistantLocation.Reputation11 },
            { "painting:ThreeInfluence", PlayerAssistantLocation.Reputation10 },
            { "Auction1:OneInfluence", PlayerAssistantLocation.Auction3 },
            { "Auction1:TwoInfluence", PlayerAssistantLocation.Auction2 },
            { "Auction1:ThreeInfluence", PlayerAssistantLocation.Auction1 },
            { "Auction3:OneInfluence", PlayerAssistantLocation.Auction6 },
            { "Auction3:TwoInfluence", PlayerAssistantLocation.Auction5 },
            { "Auction3:ThreeInfluence", PlayerAssistantLocation.Auction4 },
            { "Auction6:OneInfluence", PlayerAssistantLocation.Auction9 },
            { "Auction6:TwoInfluence", PlayerAssistantLocation.Auction8 },
            { "Auction6:ThreeInfluence", PlayerAssistantLocation.Auction7 },
        };
        public static PlayerAssistantLocation GetAssistantLocationFromIMLocation(string row, string column)
        {
            var key = row + ':' + column;
            if ( IMLocationToAssistantLocation.ContainsKey(key) )
                return IMLocationToAssistantLocation[row + ':' + column];
            return PlayerAssistantLocation.Office;
        }

        public static PlayerAssistant GetAssistantByIMLocation(Game game, string row, string column)
        {
            var location = GetAssistantLocationFromIMLocation(row, column);
            if (location == PlayerAssistantLocation.Office)
                return null;
            return game.Assistants.FirstOrDefault(a => a.Location == location);
        }

        internal static void MoveAssistantToOffice(PlayerAssistant assistant)
        {
            var officeCount = assistant.Player.Assistants.Where(a => a.Location == PlayerAssistantLocation.Office).Count();
            if (officeCount < 4)
                assistant.Location = PlayerAssistantLocation.Office;
            else
                assistant.Location = PlayerAssistantLocation.OutOfPlay;
        }
    }
}