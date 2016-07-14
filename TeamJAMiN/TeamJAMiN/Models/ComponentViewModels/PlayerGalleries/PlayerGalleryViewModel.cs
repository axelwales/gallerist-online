using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class PlayerGalleryViewModel
    {
        public PlayerGalleryViewModel(string userName, Game game, PlayerColor color)
        {
            Game = game;
            Color = color;

            var player = game.Players.FirstOrDefault(p => p.Color == color);
            HasPlayer = player != null;
            EmptyGalleryCssClass = HasPlayer ? "" : "unused-gallery-region";
            IsGalleryFirst = color == PlayerColor.yellow || color == PlayerColor.purple;

            Gallery = new VisitorDisplayViewModel(userName, game, player, GameVisitorLocation.Gallery);
            Lobby = new VisitorDisplayViewModel(userName, game, player, GameVisitorLocation.Lobby);
        }

        public Game Game { get; private set; }
        public PlayerColor Color { get; private set; }

        public bool HasPlayer { get; private set; }
        public string EmptyGalleryCssClass { get; private set; }
        public bool IsGalleryFirst { get; private set; }

        public VisitorDisplayViewModel Gallery { get; private set; }
        public VisitorDisplayViewModel Lobby { get; private set; }
    }
}