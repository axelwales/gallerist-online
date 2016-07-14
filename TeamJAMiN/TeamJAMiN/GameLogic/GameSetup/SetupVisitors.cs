using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameControllerHelpers
{
    public static class SetupVisitors
    {
        public static void ChooseVisitors(this Game newGame)
        {
            var visitorList = new List<GameVisitor> {
                new GameVisitor { Type = VisitorTicketType.collector, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.collector, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.collector, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.collector, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag }
            };
            if (newGame.Players.Count() == 4)
            {
                visitorList.AddRange(
                    new List<GameVisitor> {
                        new GameVisitor { Type = VisitorTicketType.collector, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.collector, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.collector, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.collector, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                    }
                );
            }
            else if (newGame.Players.Count() == 3)
            {
                visitorList.AddRange(
                    new HashSet<GameVisitor> {
                        new GameVisitor { Type = VisitorTicketType.collector, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.collector, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.vip, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                        new GameVisitor { Type = VisitorTicketType.investor, Location = GameVisitorLocation.Bag },
                    }
                );
            }
            visitorList = visitorList.Shuffle().ToList();
            var i = 0;
            foreach (GameVisitor visitor in visitorList)
            {
                visitor.Order = i++;
                visitor.PlayerGallery = PlayerColor.none;
            }
            newGame.Visitors = new HashSet<GameVisitor>(visitorList);
        }
        public static void DrawInitialVisitors(this Game newGame)
        {
            foreach (ArtType type in Enum.GetValues(typeof(ArtType)))
            {
                newGame.SetupNextArt(type);
            }
            newGame.DrawVisitors(4).UpdateVisitorLocation(GameVisitorLocation.Plaza, PlayerColor.none);
            foreach (Player player in newGame.Players)
            {
                newGame.DrawVisitors(1).UpdateVisitorLocation(GameVisitorLocation.Lobby, player.Color);
            }
        }
        public static void SetupTickets(this Game newGame)
        {
            var numTickets = 10;
            if (newGame.Players.Count() == 4)
                numTickets = 20;
            else if (newGame.Players.Count() == 3)
                numTickets = 15;
            newGame.AvailableCollectorTickets = numTickets;
            newGame.AvailableInvestorTickets = numTickets;
            newGame.AvailableVipTickets = numTickets;
        }
    }
}