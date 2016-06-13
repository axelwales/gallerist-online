using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class GameArt
    {
        public GameArt(TemplateArt temp)
        {
            Type = temp.Type;
            Fame = temp.Fame;
            NumTickets = temp.NumTickets;
            FirstTicket = temp.FirstTicket;
            if(temp.NumTickets >= 2)
                SecondTicket = temp.SecondTicket;
            else
                SecondTicket = new VisitorTicketType[] { };
        }

        public GameArt()
        {
            FirstTicket = new VisitorTicketType[] { };
            SecondTicket = new VisitorTicketType[] { };
        }

        public int Id { get; set; }  
        public ArtType Type { get; set; }
        public GameArtist Artist { get; set; }
        public int Fame { get; set; }
        public int NumTickets { get; set; }
        public int Order { get; set; }
        public bool IsSold { get; set; }

        public string FirstTicketData { get; set; }
        public VisitorTicketType[] FirstTicket
        {
            get
            {

                return Array.ConvertAll(FirstTicketData.Split(';'), v => (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), v));
            }
            set
            {
                FirstTicketData = String.Join(";", value.Select(v => v.ToString()).ToArray());
            }
        }
        public string SecondTicketData { get; set; }
        public VisitorTicketType[] SecondTicket
        {
            get
            {
                if (SecondTicketData == null || SecondTicketData == "")
                {
                    return null;
                }
                return Array.ConvertAll(SecondTicketData.Split(';'), v => (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), v));
            }
            set
            {
                SecondTicketData = String.Join(";", value.Select(v => v.ToString()).ToArray());
            }
        }
    }
}
