using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class ArtManager
    {
        public static int[] StarToMoney = new int[] { 0, 5, 8, 11, 14, 17, 20 };
        public static void SetupNextArt(this Game game, ArtType type)
        {
            var art = game.GetArtFromStack(type);
            if(art != null)
            {
                game.VisitorToArtStack(art.Type, art.NumTickets);
            }
        }

        public static bool ValidateArtistLocationString (this GameAction action)
        {
            var locationParams = action.Location.Split(':');
            if( locationParams.Count() != 2)
            {
                return false;
            }

            if ( !Enum.IsDefined(typeof(ArtType),locationParams[0] ))
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(ArtistCategory), locationParams[1]))
            {
                return false;
            }

            return true;
        }

        public static GameArtist GetArtistByLocationString (this Game game, string location)
        {
            var locationParams = location.Split(':');
            var type = (ArtType)Enum.Parse(typeof(ArtType), locationParams[0]);
            var category = (ArtistCategory)Enum.Parse(typeof(ArtistCategory), locationParams[1]);
            return game.Artists.First(a => a.ArtType == type && a.Category == category);
        }

        public static GameArt GetArtFromStack(this Game game, ArtType type)
        {
            //todo check if stack is empty
            return game.GetArtStack(type).FirstOrDefault();            
        }

        public static List<GameArt> GetArtStack(this Game game, ArtType type)
        {
            return game.Art.Where(a => a.Type == type && a.Artist == null).OrderBy(a => a.Order).ToList();
        }

        public static int GetArtValue(this GameArt art)
        {
            var artist = art.Artist;
            var fame = artist.Fame;
            var starLevel = 0;
            for (starLevel = 0; starLevel < artist.StarLevels.Count()+1; starLevel++)
            {
                if(starLevel == artist.StarLevels.Count() || fame < artist.StarLevels[starLevel])
                {
                    break;
                }
            }
            return StarToMoney[starLevel];
        }

        public static List<GameActionState> GetArtTicketActionStates(this GameArt art)
        {
            var result = new List<GameActionState>();
            result.Add(TicketManager.GetStateByTicketList(art.FirstTicket.ToList()));
            if(art.SecondTicket != null && art.SecondTicket.Count() > 0)
            {
                result.Add(TicketManager.GetStateByTicketList(art.SecondTicket.ToList()));
            }
            return result;
        }
    }
}