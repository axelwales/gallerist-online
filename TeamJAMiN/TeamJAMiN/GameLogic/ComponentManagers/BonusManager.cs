using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.GameLogic.ComponentManagers
{
    public class BonusManager
    {
        public static Dictionary<BonusType, GameActionState> BonusTypeToState = new Dictionary<BonusType, GameActionState>
        {
            { BonusType.assistant, GameActionState.GetAssistant },
            { BonusType.twoTickets, GameActionState.ChooseTicketAnyTwo },
            { BonusType.money, GameActionState.GetMoney },
            { BonusType.influence, GameActionState.GetInfluence },
            { BonusType.fame, GameActionState.ChooseArtistFame },
            { BonusType.plazaVisitor, GameActionState.ChooseVisitorFromPlaza }
        };

        public static Dictionary<GameActionState, bool> BonusStateIsExecutable = new Dictionary<GameActionState, bool>
        {
            { GameActionState.GetAssistant, true },
            { GameActionState.GetMoney, true },
            { GameActionState.GetInfluence, true },
            { GameActionState.GetFame, true },
            { GameActionState.GetTicketCollector, true },
            { GameActionState.GetTicketInvestor, true },
            { GameActionState.GetTicketVip, true },
            { GameActionState.ChooseArtistFame, false },
            { GameActionState.ChooseTicketAnyTwo, false },
            { GameActionState.ChooseTicketAny, false },
            { GameActionState.ChooseVisitorFromPlaza, false },
            { GameActionState.ChooseContract, false },
            { GameActionState.ChooseTicketCollectorVip, false },
            { GameActionState.ChooseTicketCollectorInvestor, false },
            { GameActionState.ChooseTicketToThrowAway, false },
            { GameActionState.ChooseVisitorFromPlazaVipInvestor, false },
            { GameActionState.ChooseVisitorFromBag, false },
        };

        public static GameActionState[] PromotionBonusStateByLevel = 
        {
            GameActionState.ChooseTicketAny,
            GameActionState.GetInfluence,
            GameActionState.ChooseTicketAnyTwo,
            GameActionState.GetMoney,
            GameActionState.ChooseVisitorFromPlaza
        };
    }
}