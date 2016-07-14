using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class ArtViewModel
    {
        public ArtViewModel(Game game, GameArt art)
        {
            Art = art;
            Visitors = game.VisitorByLocation(TypeLocationMap.TypeToLocation[art.Type]);
            VisitorTypeNames = Visitors.Select(v => v.Type.ToString()).ToArray();
            SetTicketStrings(art);
        }

        public GameArt Art { get; private set; }

        private List<GameVisitor> Visitors { get; set; }
        public string[] VisitorTypeNames { get; private set; } //Projection of Visitors to visitor type for html

        public string FirstTicket { get; private set; }
        public string SecondTicket { get; private set; }

        public void SetTicketStrings(GameArt art)
        {
            FirstTicket = ArtTicketToClass(Art.FirstTicket);
            SecondTicket = "";
            if (Art.NumTickets > 1)
            {
                SecondTicket = ArtTicketToClass(Art.SecondTicket);
            }
            if (SecondTicket == FirstTicket)
            {
                FirstTicket = "art-tile-ticket-single " + "ticket-any-two";
                SecondTicket = "hidden";
            }
            else if (SecondTicket != "")
            {
                FirstTicket = "art-tile-ticket " + FirstTicket;
                SecondTicket = "art-tile-ticket " + SecondTicket;
            }
            else
            {
                FirstTicket = "art-tile-ticket-single " + FirstTicket;
                SecondTicket = "hidden";
            }
        }

        private string ArtTicketToClass(VisitorTicketType[] ticketList)
        {
            if (ticketList.Count() == 3)
            {
                return "ticket-any";
            }
            string[] stringArray = { "", "", "" };
            foreach (VisitorTicketType ticket in ticketList)
            {
                if (ticket == VisitorTicketType.investor)
                {
                    stringArray[0] = "investor";
                }
                if (ticket == VisitorTicketType.collector)
                {
                    stringArray[1] = "collector";
                }
                if (ticket == VisitorTicketType.vip)
                {
                    stringArray[2] = "vip";
                }
            }
            var result = "ticket";
            foreach (string s in stringArray)
            {
                if (s != "")
                    result += "-" + s;
            }
            return result;
        }
    }
}