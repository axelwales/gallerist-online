using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class ArtistColonyViewModel
    {
        public ArtistColonyViewModel(string userName, Game game)
        {
            Game = game;

            SetArtistLists(userName, game);
            SetArtList(game);
        }

        public Game Game { get; private set; }

        public static List<ArtType> ArtTypeList =  new List<ArtType> { ArtType.digital, ArtType.painting, ArtType.sculpture, ArtType.photo };

        public List<ArtistViewModel> BlueArtists { get; private set; }
        public List<ArtistViewModel> RedArtists { get; private set; }

        public List<GameArt> Art { get; private set; }

        private void SetArtistLists(string userName, Game game)
        {
            foreach (ArtistCategory category in new List<ArtistCategory> { ArtistCategory.blue, ArtistCategory.red })
            {
                var currentState = game.CurrentTurn.CurrentAction.State;
                var result = new List<ArtistViewModel>();
                foreach (ArtType type in ArtTypeList)
                {
                    var artist = game.Artists.Where(a => a.ArtType == type && a.Category == category).First();
                    result.Add(new ArtistViewModel(userName, game, artist, currentState));
                }
                if (category == ArtistCategory.blue) BlueArtists = result;
                else RedArtists = result;
            }
        }

        private void SetArtList(Game game)
        {
            var result = new List<GameArt>();
            foreach(ArtType type in ArtTypeList)
            {
                var art = game.GetArtFromStack(type);
                result.Add(art);
            }
            Art = result;
        }
    }
}