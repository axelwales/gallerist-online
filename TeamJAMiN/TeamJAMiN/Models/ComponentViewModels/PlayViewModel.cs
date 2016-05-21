using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.ComponentViewModels;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class PlayViewModel
    {
        public PlayViewModel(string userName, Game game)
        {
            Game = game;

            SalesOfficeModel = new SalesOfficeViewModel(userName, game);
            InternationalMarketModel = new InternationalMarketViewModel(userName, game);
            ArtistColonyModel = new ArtistColonyViewModel(userName, game);
            TicketStacksModel = new TicketStacksViewModel(userName, game);

            InfluenceTrackModel = new InfluenceTrackViewModel(game);

            SetLocationViewModels(userName, game);
            SetGalleryModels(game);

            SetPlazaCounts(game);

            SetBoardModels(userName, game);
        }

        private void SetBoardModels(string userName, Game game)
        {
            var result = new List<PlayerBoardViewModel>();
            foreach(Player player in game.Players)
            {
                result.Add(new PlayerBoardViewModel(userName, player));
            }
            PlayerBoardModels = result;
        }

        private void SetPlazaCounts(Game game)
        {
            var plazaVisitors = game.VisitorByLocation(GameVisitorLocation.Plaza);
            var plazaInvestorCount = plazaVisitors.Where(v => v.Type == VisitorTicketType.investor).Count();
            var plazaCollectorCount = plazaVisitors.Where(v => v.Type == VisitorTicketType.collector).Count();
            var plazaVipCount = plazaVisitors.Where(v => v.Type == VisitorTicketType.vip).Count();
            PlazaVisitorCounts = new List<int> { plazaInvestorCount, plazaCollectorCount, plazaVipCount };
        }

        private void SetGalleryModels(Game game)
        {
            var result = new List<PlayerGalleryViewModel>();
            foreach (PlayerColor color in TopGalleryOrder )
            {
                result.Add(new PlayerGalleryViewModel(game, color));
            }
            TopGalleryModels = result;
            result = new List<PlayerGalleryViewModel>();
            foreach (PlayerColor color in BottomGalleryOrder)
            {
                result.Add(new PlayerGalleryViewModel(game, color));
            }
            BottomGalleryModels = result;
        }

        private void SetLocationViewModels(string userName, Game game)
        {
            var result = new List<ActionSpaceViewModel>();
            foreach ( PlayerLocation location in ActionList)
            {
                result.Add(new ActionSpaceViewModel(userName, game, location));
            }
            LocationViewModels = result;
        }

        public Game Game { get; private set; }

        public SalesOfficeViewModel SalesOfficeModel { get; private set; }
        public InternationalMarketViewModel InternationalMarketModel { get; private set; }
        public ArtistColonyViewModel ArtistColonyModel { get; private set; }
        public TicketStacksViewModel TicketStacksModel { get; private set; }

        public InfluenceTrackViewModel InfluenceTrackModel { get; private set; }

        private List<PlayerLocation> ActionList = new List<PlayerLocation> { PlayerLocation.SalesOffice, PlayerLocation.ArtistColony, PlayerLocation.InternationalMarket, PlayerLocation.MediaCenter };
        public List<ActionSpaceViewModel> LocationViewModels { get; private set; }

        private List<PlayerColor> TopGalleryOrder = new List<PlayerColor> { PlayerColor.yellow, PlayerColor.purple };
        public List<PlayerGalleryViewModel> TopGalleryModels { get; private set; }

        private List<PlayerColor> BottomGalleryOrder = new List<PlayerColor> { PlayerColor.blue, PlayerColor.orange };
        public List<PlayerGalleryViewModel> BottomGalleryModels { get; private set; }

        public List<int> PlazaVisitorCounts { get; private set; }

        public List<PlayerBoardViewModel> PlayerBoardModels { get; private set; }
    }

}