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
            { BonusType.fame, GameActionState.GetFame }
        };

        public static Dictionary<GameActionState, bool> BonusStateIsExecutable = new Dictionary<GameActionState, bool>
        {
            { GameActionState.GetAssistant, true },
            { GameActionState.ChooseTicketAnyTwo, false },
            { GameActionState.GetMoney, true },
            { GameActionState.GetInfluence, true },
            { GameActionState.GetFame, true }
        };
    }
}