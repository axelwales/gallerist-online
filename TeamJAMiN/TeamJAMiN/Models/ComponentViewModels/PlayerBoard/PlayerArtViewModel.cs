using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class PlayerArtViewModel
    {
        public GameArt Art { get; private set; }
        public string ArtValue { get; private set; }

        public bool HasActionForm { get; private set; }
        public bool IsPlayerBoardOfActivePlayer { get; private set; }

        public string Location { get; private set; }
        public GameActionState State = GameActionState.SellChooseArt;

        public PlayerArtViewModel(string userName, Player player, GameArt art)
        {
            Art = art;
            ArtValue = art.GetArtValue().ToString();
            Location = art.Order.ToString();

            IsPlayerBoardOfActivePlayer = player.Id == player.Game.CurrentPlayer.Id;

            HasActionForm = FormHelper.HasActionForm(userName, player.Game, State, Location, IsPlayerBoardOfActivePlayer);
        }
    }
}