using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class TicketManager
    {
        public static void GetTicket(this Player player, VisitorTicketType type)
        {
            switch(type)
            {
                case VisitorTicketType.collector:
                    player.GetTicketCollector();
                    break;
                case VisitorTicketType.investor:
                    player.GetTicketInvestor();
                    break;
                case VisitorTicketType.vip:
                    player.GetTicketVip();
                    break;
                default:
                    break;

            }
        }
        public static int GetAvailableTicketsByType(this Game game, VisitorTicketType type)
        {
            switch (type)
            {
                case VisitorTicketType.collector:
                    return game.AvailableCollectorTickets;
                case VisitorTicketType.investor:
                    return game.AvailableInvestorTickets;
                case VisitorTicketType.vip:
                    return game.AvailableVipTickets;
                default:
                    return -1;
            }
        }

        public static int GetPlayerTicketCountByType(this Player player, VisitorTicketType type)
        {
            switch (type)
            {
                case VisitorTicketType.collector:
                    return player.CollectorTickets;
                case VisitorTicketType.investor:
                    return player.InvestorTickets;
                case VisitorTicketType.vip:
                    return player.VipTickets;
                default:
                    return -1;
            }
        }

        public static void RemoveTicketByType(this Game game, VisitorTicketType type)
        {
            if( game.GetAvailableTicketsByType(type) != 0 )
            {
                switch (type)
                {
                    case VisitorTicketType.collector:
                        game.AvailableCollectorTickets--;
                        break;
                    case VisitorTicketType.investor:
                        game.AvailableInvestorTickets--;
                        break;
                    case VisitorTicketType.vip:
                        game.AvailableVipTickets--;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void GetTicketCollector(this Player player)
        {
            if(player.Game.AvailableCollectorTickets > 0)
                player.Game.AvailableCollectorTickets -= 1;
            player.CollectorTickets += 1;
        }
        public static void GetTicketInvestor(this Player player)
        {
            if (player.Game.AvailableInvestorTickets > 0)
                player.Game.AvailableInvestorTickets -= 1;
            player.InvestorTickets += 1;
        }
        public static void GetTicketVip(this Player player)
        {
            if (player.Game.AvailableVipTickets > 0)
                player.Game.AvailableVipTickets -= 1;
            player.VipTickets += 1;
        }

        public static GameActionState GetStateByTicketList(List<VisitorTicketType> ticketList)
        {
            if (ticketList.Count() == 3)
            {
                return GameActionState.ChooseTicketAny;
            }
            if (ticketList.Count() == 1)
            {
                var ticket = ticketList.Single();
                if (ticket == VisitorTicketType.investor)
                {
                    return GameActionState.GetTicketInvestor;
                }
                if (ticket == VisitorTicketType.collector)
                {
                    return GameActionState.GetTicketCollector;
                }
                if (ticket == VisitorTicketType.vip)
                {
                    return GameActionState.GetTicketVip;
                }
            }
            string[] stringArray = { "", "" };
            foreach (VisitorTicketType ticket in ticketList)
            {
                if (ticket == VisitorTicketType.investor)
                {
                    stringArray[1] = "Investor";
                }
                if (ticket == VisitorTicketType.collector)
                {
                    stringArray[0] = "Collector";
                }
                if (ticket == VisitorTicketType.vip)
                {
                    stringArray[1] = "Vip";
                }
            }
            string result = "ChooseTicket";
            foreach (string s in stringArray)
            {
                if (s != "")
                    result += s;
            }
            return (GameActionState)Enum.Parse(typeof(GameActionState), result);
        }
    }
}