using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class PlayerContractsViewModel
    {
        public PlayerContractsViewModel(string userName, Player player)
        {
            IsPlayerBoardOfActivePlayer = player.Id == player.Game.CurrentPlayer.Id;
            IsActivePlayer = FormHelper.IsActivePlayer(userName, player.Game);
            IsValidActionState = player.Game.CurrentTurn.CurrentAction.State == GameActionState.ContractDraft;

            Player = player;
            State = GameActionState.ContractToPlayerBoard;

            SetContracts(player);

        }

        public bool IsPlayerBoardOfActivePlayer { get; private set; }
        public bool IsActivePlayer { get; private set; }
        public bool IsValidActionState { get; private set; }

        public Player Player { get; private set; }
        public GameActionState State { get; private set; }

        private List<GameContractLocation> LocationOrder = new List<GameContractLocation> { GameContractLocation.Investor, GameContractLocation.Vip, GameContractLocation.Any };
        public List<PlayerContractViewModel> Contracts { get; private set; }

        private void SetContracts(Player player)
        {
            var result = new List<PlayerContractViewModel>();
            foreach ( GameContractLocation location in LocationOrder )
            {
                var contract = player.GetContractAtLocation(location);
                var dto = new PlayerContractViewModel(contract, location);
                result.Add(dto);
            }
            Contracts = result;
        }
                
    }

    public class PlayerContractViewModel
    {
        public PlayerContractViewModel(GameContract contract, GameContractLocation location)
        {
            Contract = contract;
            Location = location;
            if (contract == null)
            {
                Ticket = location.ToString().ToLower();
                EmptyCssClass = "player-contract-empty";
                BonusClass = "";
            }
            else
            {
                Ticket = "";
                EmptyCssClass = "";
                BonusClass = IconCss.BonusClass[contract.Bonus];
            }
        }

        public GameContract Contract { get; private set; }
        public GameContractLocation Location { get; private set; }
        public string Ticket { get; private set; }
        public string EmptyCssClass { get; private set; }
        public string BonusClass { get; private set; }
    }
}