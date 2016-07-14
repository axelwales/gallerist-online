using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class PlayerReputationTileLocationViewModel
    {
        public PlayerReputationTileLocationViewModel(string userName, Player player, GameReputationTileLocation location)
        {
            IsPlayerBoardOfActivePlayer = player.Id == player.Game.CurrentPlayer.Id;
            IsActivePlayer = FormHelper.IsActivePlayer(userName, player.Game);
            IsValidActionState = player.Game.CurrentTurn.CurrentAction.State == GameActionState.Reputation;

            Player = player;
            State = GameActionState.ReputationToBoard;
            Location = location;
            Tile = player.Tiles.FirstOrDefault(t => t.Column == location);
            BonusClass = IconCss.BonusClass[IconCss.PlayerReputationLocationToBonus[location]];
        }

        public bool IsPlayerBoardOfActivePlayer { get; private set; }
        public bool IsActivePlayer { get; private set; }
        public bool IsValidActionState { get; private set; }

        public Player Player { get; private set; }
        public GameActionState State { get; private set; }

        public GameReputationTileLocation Location { get; private set; }
        public GameReputationTile Tile { get; private set; }
        public string BonusClass { get; private set; }
    }
}