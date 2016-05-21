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
        public ArtistViewModel(GameArtist artist, GameActionState currentState)
        {
            Artist = artist;
            SetFormAction(artist, currentState);
            SetStarProperties(artist);
            SetPromotionText(artist);
            BonusClass = IconCss.BonusClass[artist.DiscoverBonus];
        }

        private void SetFormAction(GameArtist artist, GameActionState currentState)
        {
            if (currentState == GameActionState.ArtistColony)
            {
                if (artist.IsDiscovered)
                {
                    FormAction = GameActionState.ArtBuy;
                }
                else
                    FormAction = GameActionState.ArtistDiscover;
            }
            else
                FormAction = GameActionState.Promote;
        }

        public GameActionState FormAction { get; private set; }

        private static string[] StarClass = { "", "star-green-1", "star-green-2", "star-green-3", "star-gold-1", "star-gold-2", "star-celebrity" };

        public GameArtist Artist { get; private set; }

        public string CurrentStar { get; private set; }
        public string NextStar { get; private set; }
        public int FameAtNextStar { get; private set; }

        public string PromotionText { get; private set; }

        public string BonusClass { get; private set; }

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
            while (artist.StarLevels[i] < artist.Fame) { i++; }
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
    }
}