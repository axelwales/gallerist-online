using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class UnemployedAssistantViewModel
    {
        public UnemployedAssistantViewModel(string userName, Player player, int index )
        {
            IsPlayerBoardOfActivePlayer = player.Id == player.Game.CurrentPlayer.Id;
            IsActivePlayer = FormHelper.IsActivePlayer(userName, player.Game);
            IsValidActionState = player.Game.CurrentTurn.CurrentAction.State == GameActionState.MediaCenter;
            ActionLocation = index.ToString();

            Player = player;
            State = GameActionState.Hire;

            AssistantIndex = index;
            AssistantCost = AssistantManager.AssistantCost[index];
            AssistantBonus = IconCss.ActionStateToBonusClass[AssistantManager.AssistantBonus[index]];
            IsAvailable = player.Assistants.Count - 2 <= index;
            SetAssistantCssClass(IsAvailable, player);

        }

        public bool IsPlayerBoardOfActivePlayer { get; private set; }
        public bool IsActivePlayer { get; private set; }
        public bool IsValidActionState { get; private set; }
        public string ActionLocation { get; private set; }

        public Player Player { get; private set; }
        public GameActionState State { get; private set; }

        public int AssistantIndex { get; private set; }
        public int AssistantCost { get; private set; }
        public bool IsAvailable { get; private set; }

        public string AssistantCssClass { get; private set; }
        public string AssistantBonus { get; private set; }

        private void SetAssistantCssClass(bool isAvailable, Player player)
        {
            if(isAvailable)
            {
                AssistantCssClass = "player-assistant-" + player.Color;
            }
            else
            {
                AssistantCssClass = "";
            }
        }
    }
}