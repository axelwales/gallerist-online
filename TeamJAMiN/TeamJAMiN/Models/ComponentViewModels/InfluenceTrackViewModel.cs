using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class InfluenceTrackViewModel
    {
        public InfluenceTrackViewModel(Game game)
        {
            Spaces = new List<InfluenceSpaceViewModel>();
            for(int i = 0; i<36; i++)
            {
                if (game.Players.Any(p => p.Influence == i))
                {
                    var playerColors = game.Players.Where(p => p.Influence == i).Select(p => p.Color).ToList();
                    Spaces.Add(new InfluenceSpaceViewModel(i, playerColors));
                }
                else
                    Spaces.Add(new InfluenceSpaceViewModel(i));
            }
        }
        public List<InfluenceSpaceViewModel> Spaces { get; private set; }
    }

    public class InfluenceSpaceViewModel
    {
        public InfluenceSpaceViewModel(int index, List<PlayerColor> colors) : this(index)
        {
            var ulHtml = "<ul class=\"player-influence-markers\">";
            foreach (PlayerColor color in colors)
            {
                var playerMarkerHtml = "<li class=\"influence-marker influence-marker-" + color.ToString().ToLower() + "\"></li>";
                ulHtml += playerMarkerHtml;
            }
            ulHtml += "</ul>";
            InfluenceMarkerHtml = ulHtml;
        }
        public InfluenceSpaceViewModel(int index)
        {
            Index = index;
            DisplayIndex = index.ToString();
            MoneyHtml = StarHtml = KickedOutCssClass = InfluenceMarkerHtml = "";
            if (GameInfluenceTrack.InfluenceToMoney.Contains(index))
            {
                var money = Array.IndexOf(GameInfluenceTrack.InfluenceToMoney, index);
                MoneyHtml = @"<div class=""influence-money-icon money-" + money + @"""></div>";
            }
            if (index % 5 == 0 && index != 35)
            {
                StarHtml = @"<div class=""star-white influence-star-icon""></div>";
                KickedOutCssClass = "has-star ";
            }
            if (index % 5 == 0 && index != 0)
            {
                KickedOutCssClass += "influence-kicked-out";
            }
            else if (index == 0)
            {
                DisplayIndex = "";
                KickedOutCssClass += "influence-icon";
            }
        }
        public int Index { get; private set; }
        public string DisplayIndex { get; private set; }
        public string MoneyHtml { get; private set; }
        public string StarHtml { get; private set; }
        public string KickedOutCssClass { get; private set; }
        public string InfluenceMarkerHtml { get; private set; }
    }
}