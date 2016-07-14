using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models.GameViewHelpers;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class InfluenceTrackViewModel
    {
        public InfluenceTrackViewModel(string userName, Game game)
        {
            Game = game;
            Spaces = new List<InfluenceSpaceViewModel>();
            var currentPlayerInfluence = game.CurrentPlayer.Influence;

            for(int i = 0; i<36; i++)
            {
                if (game.Players.Any(p => p.Influence == i))
                {
                    var playerColors = game.Players.Where(p => p.Influence == i).Select(p => p.Color).ToList();
                    Spaces.Add(new InfluenceSpaceViewModel(i, currentPlayerInfluence, userName, game, playerColors ));
                }
                else
                    Spaces.Add(new InfluenceSpaceViewModel(i, currentPlayerInfluence, userName, game));
            }
        }

        public Game Game { get; private set; }
        public List<InfluenceSpaceViewModel> Spaces { get; private set; }
    }

    public class InfluenceSpaceViewModel
    {
        public InfluenceSpaceViewModel(int index, int  currentPlayerInfluence, string userName, Game game, List<PlayerColor> colors) : this(index, currentPlayerInfluence, userName, game)
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
        public InfluenceSpaceViewModel(int index, int currentPlayerInfluence, string userName, Game game)
        {
            Index = index;
            DisplayIndex = Location = index.ToString();
            MoneyHtml = StarHtml = KickedOutCssClass = InfluenceMarkerHtml = "";
            UseInfluenceAsMoney = UseInfluenceAsFame = false;

            SetPropertiesByIndex(Index, currentPlayerInfluence);

            SetStateAndLocation(Index, currentPlayerInfluence, game);
            HasActionForm = FormHelper.HasActionForm(userName, game, State, Location, State != GameActionState.NoAction);
        }

        private void SetStateAndLocation(int index, int currentPlayerInfluence, Game game)
        {
            State = GameActionState.NoAction;
            var actions = TurnManager.GetNextActions(game.CurrentTurn);
            if( actions != null )
            {
                var action = actions.First();
                if (action.State == GameActionState.UseInfluenceAsMoney && UseInfluenceAsMoney)
                {
                    State = GameActionState.UseInfluenceAsMoney;
                    Location = InfluenceManager.CalculateMoneySpentByInfluence(currentPlayerInfluence, index).ToString();
                }
                else if (action.State == GameActionState.UseInfluenceAsFame && UseInfluenceAsFame)
                {
                    State = GameActionState.UseInfluenceAsFame;
                    Location = InfluenceManager.CalculateFameGainedByInfluence(currentPlayerInfluence, index).ToString();
                }

            }
        }

        private void SetPropertiesByIndex(int index, int currentPlayerInfluence)
        {
            if (InfluenceManager.InfluenceToMoney.Contains(index))
            {
                var money = Array.IndexOf(InfluenceManager.InfluenceToMoney, index);
                MoneyHtml = @"<div class=""influence-money-icon money-" + money + @"""></div>";
                if (index < currentPlayerInfluence)
                {
                    UseInfluenceAsMoney = true;
                }

            }
            if (index % 5 == 0 && index != 35)
            {
                StarHtml = @"<div class=""star-white influence-star-icon""></div>";
                KickedOutCssClass = "has-star ";
                if (index < currentPlayerInfluence)
                {
                    UseInfluenceAsFame = true;
                }
            }
            if (index == currentPlayerInfluence)
            {
                UseInfluenceAsFame = true;
                UseInfluenceAsMoney = true;
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

        public bool UseInfluenceAsMoney { get; private set; }
        public bool UseInfluenceAsFame { get; private set; }

        public bool HasActionForm { get; private set; }
        public GameActionState State { get; private set; }
        public string Location { get; private set; }
    }
}