using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class VisitorManager
    {
        public static HashSet<GameVisitorLocation> NeutralLocations = new HashSet<GameVisitorLocation>
        {
            GameVisitorLocation.Plaza, GameVisitorLocation.Bag, GameVisitorLocation.Digital, GameVisitorLocation.Painting, GameVisitorLocation.Photo, GameVisitorLocation.Sculpture
        };

        public static List<GameVisitor> DrawVisitors(this Game game, int count)
        {
            return game.Visitors.Where(v => v.Location == GameVisitorLocation.Bag).OrderBy(v => v.Order).Take(count).ToList();
        }

        public static void VisitorToArtStack(this Game game, ArtType type, int count)
        {
            game.DrawVisitors(count).UpdateVisitorLocation(TypeLocationMap.TypeToLocation[type], PlayerColor.none);            
        }

        public static void MoveFromArtStackToPlaza(this Game game, ArtType type)
        {
            game.Visitors.Where(v => v.Location == TypeLocationMap.TypeToLocation[type]).UpdateVisitorLocation(GameVisitorLocation.Plaza, PlayerColor.none);
        }

        public static void UpdateVisitorLocation(this IEnumerable<GameVisitor> list, GameVisitorLocation location, PlayerColor color)
        {
            foreach (GameVisitor v in list)
            {
                v.UpdateVisitorLocation(location, color);
            }
        }

        public static void UpdateVisitorLocation(this GameVisitor visitor, GameVisitorLocation location, PlayerColor color)
        {
            visitor.Location = location;
            visitor.PlayerGallery = color;
        }

        public static int GetGalleryVisitorCountByType(this Player player, VisitorTicketType type)
        {
            return GetVisitorCount(player.Game, type, player.Color, GameVisitorLocation.Gallery);
        }

        public static int GetLobbyVisitorCountByType(this Player player, VisitorTicketType type)
        {
            return GetVisitorCount(player.Game, type, player.Color, GameVisitorLocation.Lobby);
        }

        public static int GetPlazaVisitorCountByType(this Game game, VisitorTicketType type)
        {
            return GetVisitorCount(game, type, PlayerColor.none, GameVisitorLocation.Plaza);
        }

        public static int GetVisitorCountAtLocation(Game game, PlayerColor color, GameVisitorLocation location)
        {
            return game.Visitors.Where(v => v.PlayerGallery == color && v.Location == location).Count();
        }

        public static int GetVisitorCount(Game game, VisitorTicketType type, PlayerColor color, GameVisitorLocation location)
        {
            return game.Visitors.Where(v => v.Type == type && v.PlayerGallery == color && v.Location == location).Count();
        }

        public static int GetBagVisitorCountByType(this Game game, VisitorTicketType type)
        {
            return game.Visitors.Where(v => v.Type == type && v.Location == GameVisitorLocation.Bag).Count();
        }

        public static void MoveVisitorPlazaToGallery(this Player player, VisitorTicketType type)
        {
            player.MoveVisitor(type, GameVisitorLocation.Plaza, GameVisitorLocation.Gallery);
        }

        public static void MoveVisitorBagToGallery(this Player player, VisitorTicketType type)
        {
            player.MoveVisitor(type, GameVisitorLocation.Bag, GameVisitorLocation.Gallery);
        }

        public static void MoveVisitor(this Player player, VisitorTicketType type, GameVisitorLocation start, GameVisitorLocation end)
        {
            var visitor = player.Game.Visitors.FirstOrDefault(v => v.Type == type && v.Location == start);
            if (visitor != null)
            {
                var color = player.Color;
                if (NeutralLocations.Contains(end))
                    color = PlayerColor.none;

                visitor.UpdateVisitorLocation(end, color);
            }
        }

        public static bool ValidateVisitorLocationString(string location)
        {
            var locationParams = location.Split(':');
            if (locationParams.Count() != 3)
            {
                return false;
            }
            if (!Enum.IsDefined(typeof(VisitorTicketType), locationParams[0]))
            {
                return false;
            }
            if (!Enum.IsDefined(typeof(GameVisitorLocation), locationParams[1]))
            {
                return false;
            }
            if (!Enum.IsDefined(typeof(PlayerColor), locationParams[2]))
            {
                return false;
            }

            return true;
        }

        public static VisitorTicketType GetVisitorTypeFromLocationString(string location)
        {
            var locationParams = location.Split(':');
            return (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), locationParams[0]);
        }
        public static GameVisitorLocation GetVisitorLocationFromLocationString(string location)
        {
            var locationParams = location.Split(':');
            return (GameVisitorLocation)Enum.Parse(typeof(GameVisitorLocation), locationParams[1]);
        }
        public static PlayerColor GetPlayerColorFromVisitorLocationString(string location)
        {
            var locationParams = location.Split(':');
            return (PlayerColor)Enum.Parse(typeof(PlayerColor), locationParams[2]);
        }

    }
    public static class TypeLocationMap
    {
        public static Dictionary<ArtType, GameVisitorLocation> TypeToLocation = new Dictionary<ArtType, GameVisitorLocation>
        {
            { ArtType.photo, GameVisitorLocation.Photo },
            { ArtType.painting, GameVisitorLocation.Painting },
            { ArtType.digital, GameVisitorLocation.Digital },
            { ArtType.sculpture, GameVisitorLocation.Sculpture },
        };
    }
}