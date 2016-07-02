using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class OfficeAssistantsViewModel
    {
        public string PlayerColor { get; private set; }
        public int NumberOfAssistants { get; private set; }

        public OfficeAssistantsViewModel(Player player)
        {
            PlayerColor = player.Color.ToString();
            NumberOfAssistants = player.Assistants.Where(a => a.Location == PlayerAssistantLocation.Office).Count();
        }
    }
}