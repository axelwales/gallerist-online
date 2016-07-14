using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.GameLogic.ActionContexts;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class ActionContextFactory
    {
        public static Dictionary<GameActionState, Type> ActionToContextType = new Dictionary<GameActionState, Type>
        {
            { GameActionState.InternationalMarket, typeof(InternationalMarketContext) },
            { GameActionState.Reputation, typeof(InternationalMarketContext) },
            { GameActionState.ReputationToBoard, typeof(InternationalMarketContext) },
            { GameActionState.MoveVisitorFromLobby, typeof(InternationalMarketContext) },
            { GameActionState.Auction, typeof(InternationalMarketContext) },
            { GameActionState.MediaCenter, typeof(MediaCenterContext) },
            { GameActionState.Promote, typeof(MediaCenterContext) },
            { GameActionState.Hire, typeof(MediaCenterContext) },
            { GameActionState.ArtistColony, typeof(ArtistColonyContext) },
            { GameActionState.ArtistDiscover, typeof(ArtistColonyContext) },
            { GameActionState.ArtBuy, typeof(ArtistColonyContext) },
            { GameActionState.SalesOffice, typeof(SalesOfficeContext) },
            { GameActionState.ContractDraft, typeof(SalesOfficeContext) },
            { GameActionState.ContractDraw, typeof(SalesOfficeContext) },
            { GameActionState.ContractToPlayerBoard, typeof(SalesOfficeContext) },
            { GameActionState.ChooseLocation, typeof(SetupContext) },
            { GameActionState.GameStart, typeof(SetupContext) },
            { GameActionState.Pass, typeof(SetupContext) },
            { GameActionState.UseInfluenceAsFame, typeof(InfluenceTrackContext) },
            { GameActionState.UseInfluenceAsMoney, typeof(InfluenceTrackContext) },
            { GameActionState.GetTicketVip, typeof(BonusContext) },
            { GameActionState.GetTicketInvestor, typeof(BonusContext) },
            { GameActionState.GetTicketCollector, typeof(BonusContext) },
            { GameActionState.GetAssistant, typeof(BonusContext) },
            { GameActionState.GetMoney, typeof(BonusContext) },
            { GameActionState.GetInfluence, typeof(BonusContext) },
            { GameActionState.GetFame, typeof(BonusContext) },
            { GameActionState.ChooseContract, typeof(BonusContext) },
            { GameActionState.ChooseTicketAny, typeof(BonusContext) },
            { GameActionState.ChooseTicketAnyTwo, typeof(BonusContext) },
            { GameActionState.ChooseTicketCollectorInvestor, typeof(BonusContext) },
            { GameActionState.ChooseTicketCollectorVip, typeof(BonusContext) },
            { GameActionState.ChooseTicketToThrowAway, typeof(BonusContext) },
            { GameActionState.ChooseVisitorFromPlaza, typeof(BonusContext) },
            { GameActionState.ChooseVisitorFromPlazaVipInvestor, typeof(BonusContext) },
            { GameActionState.ChooseVisitorFromBag, typeof(BonusContext) },
            { GameActionState.ChooseArtistFame, typeof(BonusContext) }
        };

        public static ActionContext GetContext(GameActionState state, Game game)
        {
            Type contextType = ActionToContextType[state];
            return (ActionContext)Activator.CreateInstance(contextType, game);
        }

        public static ActionContext GetContext(GameAction action, Game game)
        {
            Type contextType = ActionToContextType[action.State];
            return (ActionContext)Activator.CreateInstance(contextType, game, action);
        }
    }
}