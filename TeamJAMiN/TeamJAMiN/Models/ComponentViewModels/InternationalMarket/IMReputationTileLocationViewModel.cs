using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels.InternationalMarket
{
    public class IMReputationTileLocationViewModel
    {
        public Game Game { get; private set; }

        public bool IsValidAction { get; private set; }

        public GameActionState State = GameActionState.Reputation;
        public string Location { get; private set; }

        public ReputationTileViewModel TileViewModel { get; private set; }
        public string AssistantCssClass { get; private set; }

        public IMReputationTileLocationViewModel(string userName, Game game, ArtType row, GameReputationTileLocation column)
        {
            Game = game;

            Location = row.ToString() + ':' + column.ToString();
            IsValidAction = FormHelper.HasActionForm(userName, game, State, Location);

            var tile = game.ReputationTiles.FirstOrDefault(r => r.Row == row && r.Column == column);
            if (tile != null)
                TileViewModel = new ReputationTileViewModel(tile);
            else
            {
                TileViewModel = null;
                var assistant = AssistantManager.GetAssistantByIMLocation(game, row.ToString(), column.ToString());
                if (assistant != null)
                    AssistantCssClass = "im-assistant-icon player-assistant-" + assistant.Player.Color.ToString();
                else
                    AssistantCssClass = "";
            }

        }
    }
}