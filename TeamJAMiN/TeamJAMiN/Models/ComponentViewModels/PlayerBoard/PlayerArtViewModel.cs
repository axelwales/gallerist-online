using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class PlayerArtViewModel
    {
        public PlayerArtViewModel(GameArt art)
        {
            Art = art;
            ArtValue = art.GetArtValue().ToString();
        }
        public GameArt Art { get; private set; }
        public string ArtValue { get; private set; }
    }
}