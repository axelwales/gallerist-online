using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class TemplateArt
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public ArtType Type { get; set; }
        public int Fame { get; set; }
        public int NumTickets { get; set; }
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

                return Array.ConvertAll(SecondTicketData.Split(';'), v => (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), v));
            }
            set
            {
                SecondTicketData = String.Join(";", value.Select(v => v.ToString()).ToArray());
            }
        }
    }
}
