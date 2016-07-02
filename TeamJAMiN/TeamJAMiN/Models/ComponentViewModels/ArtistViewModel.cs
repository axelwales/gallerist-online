using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class ArtistViewModel
    {
        public bool HasFormAction { get; private set; }
        public GameActionState FormAction { get; private set; }
        public string Location { get; private set; }

        public GameArtist Artist { get; private set; }
        public string ViewString { get; private set; }

        private static string[] StarClass = { "", "star-green-1", "star-green-2", "star-green-3", "star-gold-1", "star-gold-2", "star-celebrity" };
        public string CurrentStar { get; private set; }
        public string NextStar { get; private set; }
        public int FameAtNextStar { get; private set; }

        public string PromotionText { get; private set; }

        public string BonusClass { get; private set; }

        public ArtistViewModel(string userName, Game game, GameArtist artist, GameActionState currentState)
        {
            Artist = artist;
            SetViewString(artist);

            SetFormParams(userName, game, artist, currentState);
            
            SetStarProperties(artist);
            SetPromotionText(artist);
            BonusClass = IconCss.BonusClass[artist.DiscoverBonus];
        }

        private void SetViewString(GameArtist artist)
        {
                ViewString = "~/Views/Game/ArtistColony/ArtistUndiscovered.cshtml";
                if (artist.IsDiscovered)
                    ViewString = "~/Views/Game/ArtistColony/ArtistDiscovered.cshtml";
        }

        private void SetStarProperties(GameArtist artist)
        {
            int index = GetIndexOfCurrentStarLevel(artist);
            CurrentStar = StarClass[index];
            NextStar = StarClass[index + 1];
            FameAtNextStar = artist.StarLevels[index];
        }

        private int GetIndexOfCurrentStarLevel(GameArtist artist)
        {
            int i = 0;
            while (artist.StarLevels[i] <= artist.Fame) { i++; }
            return i;
        }

        private void SetPromotionText(GameArtist artist)
        {
            var text = "-";
            if (artist.Promotion > 0)
            {
                text = artist.Promotion.ToString();
            }
            PromotionText = text;
        }

        private void SetFormParams(string userName, Game game, GameArtist artist, GameActionState currentState)
        {
            Location = artist.ArtType.ToString() + ":" + artist.Category.ToString();

            if (currentState == GameActionState.ArtistColony)
            {
                if (artist.IsDiscovered)
                    FormAction = GameActionState.ArtBuy;

                else
                    FormAction = GameActionState.ArtistDiscover;
            }
            else if (currentState == GameActionState.MediaCenter && artist.IsDiscovered)
                FormAction = GameActionState.Promote;

            else if (artist.IsDiscovered)
                FormAction = GameActionState.ChooseArtistFame;

            else
                FormAction = GameActionState.NoAction;

            HasFormAction = FormHelper.HasActionForm(userName, game, FormAction, Location, FormAction != GameActionState.NoAction);
        }
    }
}