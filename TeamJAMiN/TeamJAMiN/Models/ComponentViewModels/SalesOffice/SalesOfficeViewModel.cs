using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class SalesOfficeViewModel
    {
        public SalesOfficeViewModel(string userName, Game game)
        {
            IsActivePlayer = FormHelper.IsActivePlayer(userName, game);

            var actionManager = new ActionContextInvoker(game);
            DrawIsValidActionState = actionManager.IsValidTransition(GameActionState.ContractDraw);
            DraftIsValidActionState = actionManager.IsValidTransition(GameActionState.ContractDraft);

            Game = game;
            DrawState = GameActionState.ContractDraw;

            AllContracts = game.GetContractDecks();
            DrawCount = AllContracts[GameContractLocation.DrawDeck].Count;

            SetDraftContractModels(AllContracts);
        }

        private void SetDraftContractModels(Dictionary<GameContractLocation, List<GameContract>> allContracts)
        {
            var result = new List<DraftContractViewModel>();
            foreach(GameContractLocation location in DraftLocations)
            {
                var contract = allContracts[location].FirstOrDefault();
                result.Add( new DraftContractViewModel(contract, location));
            }
            DraftContractModels = result;
        }

        public bool IsActivePlayer { get; private set; }
        public bool DrawIsValidActionState { get; private set; }
        public bool DraftIsValidActionState { get; private set; }
        public string TooltipTitle = "Draw Cards";
        public string TooltipText = "If active you can click here to draw four (4) new contract cards";

        public Game Game { get; private set; }
        public GameActionState DrawState { get; private set; }

        public Dictionary<GameContractLocation,List<GameContract>> AllContracts { get; private set; }
        public int DrawCount { get; private set; }

        public List<GameContractLocation> DraftLocations = new List<GameContractLocation>
                { GameContractLocation.Draft0, GameContractLocation.Draft1, GameContractLocation.Draft2, GameContractLocation.Draft3 };
        public List<DraftContractViewModel> DraftContractModels;
    }
}