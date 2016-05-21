using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public enum GameActionState
    {
        GameStart, ChooseLocation, Pass, GameEnd,
        SalesOffice, ContractDraw, ContractDraft, ContractToPlayerBoard, SellChooseArt, SellChooseVisitor,
        InternationalMarket, Reputation, ReputationToBoard, Auction,
        ArtistColony, ArtistDiscover, ArtBuy,
        MediaCenter, Promote, Hire,
        ChooseTicketAny, ChooseTicketAnyTwo, ChooseTicketCollectorVip, ChooseTicketCollectorInvestor, ChooseTicketToThrowAway,
        ChooseVisitorFromPlaza, ChooseVisitorFromPlazaVipInvestor, ChooseVisitorFromBag,
        ChooseArtistFame, ChooseContract,
        GetTicketInvestor, GetMoney, GetTicketVip, GetInfluence, GetAssistant,
        GetTicketCollector,
        UseTicket,
        UseContractBonus,
        MoveVisitorStart,
        MoveVisitorEnd,
        UseInfluenceAsMoney,
        UseInfluenceAsFame,
        GetFame
    }
}
