using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class DraftContractViewModel
    {
        public DraftContractViewModel(GameContract contract, GameContractLocation location)
        {
            Contract = contract;
            BonusClass = IconCss.BonusClass[contract.Bonus];
            State = GameActionState.ContractDraft;
            Location = location;
            SetSpecificText(contract);
        }

        public GameActionState State { get; private set; }
        public GameContractLocation Location { get; private set; }

        public GameContract Contract { get; private set; }
        public string BonusClass { get; private set; }

        public string TooltipTitle = "Draft Card";
        public string TooltipText = "If active you can click here to choose this contract to place onto your player board.\n\n";
        public string TooltipSpecificText { get; set; }

        Dictionary<BonusType, string> ContractBonusToToolTip = new Dictionary<BonusType, string>
        {
            { BonusType.contract, "Choose a Contract from the Sales Office and add it to your player board." },
            { BonusType.money, "Gain Money" },
            { BonusType.influence, "Gain Influence" },
            { BonusType.bagVisitor, "Choose a Visitor from the bag and add it to your Gallery" },
            { BonusType.plazaVipInvestor, "Move a Vip or Collector from the plaza into your lobby." },
            { BonusType.assistant, "Hire the next assistant for free" },
        };

        private void SetSpecificText(GameContract contract)
        {
            TooltipSpecificText = TooltipText + "<bold>Contract Bonus</bold>: " + ContractBonusToToolTip[contract.Bonus];
        }
    }
}