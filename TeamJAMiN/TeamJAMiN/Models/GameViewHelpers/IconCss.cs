using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Models.GameViewHelpers
{
    public static class IconCss
    {
        public static Dictionary<BonusType, string> BonusClass = new Dictionary<BonusType, string>
        {
            { BonusType.plazaVisitor, "bonus-visitor-any" },
            { BonusType.money, "bonus-money" },
            { BonusType.influence, "bonus-influence" },
            { BonusType.fame, "bonus-fame" },
            { BonusType.twoTickets, "bonus-ticket-two" },
            { BonusType.assistant, "bonus-assistant" },
            { BonusType.bagVisitor, "bonus-visitor-bag" },
            { BonusType.plazaVipInvestor, "bonus-visitor-plaza-investor-vip" },
            { BonusType.ticket, "ticket-any" },
            { BonusType.contract, "bonus-contract" }
        };

        public static Dictionary<GameReputationTileLocation, BonusType> PlayerReputationLocationToBonus = new Dictionary<GameReputationTileLocation, BonusType>
        {
            { GameReputationTileLocation.Assistant, BonusType.assistant },
            { GameReputationTileLocation.Fame, BonusType.fame },
            { GameReputationTileLocation.Influence, BonusType.influence },
            { GameReputationTileLocation.Money, BonusType.money },
            { GameReputationTileLocation.Tickets, BonusType.twoTickets },
            { GameReputationTileLocation.Visitor, BonusType.plazaVisitor },
        };

        public static Dictionary<GameActionState, string> ActionStateToBonusClass = new Dictionary<GameActionState, string>
        {
            { GameActionState.NoAction, ""},
            { GameActionState.GetTicketInvestor, "ticket-investor"},
            { GameActionState.GetTicketVip, "ticket-vip"},
            { GameActionState.GetInfluence, "bonus-influence"},
            {  GameActionState.ChooseTicketAny, "ticket-any"},
            {  GameActionState.GetMoney, "bonus-money"}
        };
    }
}