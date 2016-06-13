using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class PlayerAssistant
    {
        public int Id { get; set; }
        public PlayerAssistantLocation Location { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
}
