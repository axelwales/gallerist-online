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
        public Player Player { get; private set; }

        private List<GameContractLocation> LocationOrder = new List<GameContractLocation> { GameContractLocation.Investor, GameContractLocation.Vip, GameContractLocation.Any };
        public List<PlayerContractViewModel> Contracts { get; private set; }

        public PlayerContractsViewModel(string userName, Player player)
        {
            Player = player;
            SetContracts(userName, player);
        }

        private void SetContracts(string userName, Player player)
        {
            var result = new List<PlayerContractViewModel>();
            foreach ( GameContractLocation location in LocationOrder )
            {
                var contract = player.GetContractAtLocation(location);
                var dto = new PlayerContractViewModel(userName, player, contract, location);
                result.Add(dto);
            }
            Contracts = result;
        }
                
    }

    public class PlayerContractViewModel
    {
        public bool HasActionForm { get; private set; }
        public GameActionState State { get; private set; }

        public GameContract Contract { get; private set; }
        public GameContractLocation Location { get; private set; }
        public string Ticket { get; private set; }
        public string EmptyCssClass { get; private set; }
        public string BonusClass { get; private set; }

        public PlayerContractViewModel(string userName, Player player, GameContract contract, GameContractLocation location)
        {
            Contract = contract;
            Location = location;

            bool isPlayerBoardOfActivePlayer = player.Id == player.Game.CurrentPlayer.Id;
            if (HasActionForm = FormHelper.HasActionForm(userName, player.Game, GameActionState.ContractToPlayerBoard, location.ToString(), isPlayerBoardOfActivePlayer))
                State = GameActionState.ContractToPlayerBoard;
            else if (HasActionForm = FormHelper.HasActionForm(userName, player.Game, GameActionState.UseContractBonus, location.ToString(), isPlayerBoardOfActivePlayer))
                State = GameActionState.UseContractBonus;
            else
                State = GameActionState.NoAction;

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
    }
}