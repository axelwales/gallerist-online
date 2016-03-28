﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.DataContexts;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameControllerHelpers
{
    public static class SetupGame
    {
        public static void CreateRandomSetup(this Game newGame)
        {
            using (var galleristContext = new GalleristComponentsDbContext())
            {
                //put list building on separate lines for clarity
                var artLists = galleristContext.TemplateArt.ToList().chooseArt();
                newGame.AddArtStack(artLists);

                var blueArtists = galleristContext.TemplateArtists.Where(a => a.Category == ArtistCategory.red).ToList().chooseArtists();
                var redArtists = galleristContext.TemplateArtists.Where(a => a.Category == ArtistCategory.blue).ToList().chooseArtists();
                var artBonuses = ArtColonySetup.chooseArtBonuses();

                newGame.AddArtists(blueArtists.Values.ToList(),artBonuses);
                newGame.AddArtists(redArtists.Values.ToList(),artBonuses);


                var reputationTiles = galleristContext.TemplateReputationTiles.ToList().chooseReputationTiles();
                newGame.AddReputationTiles(reputationTiles);

                var contracts = galleristContext.TemplateContracts.ToList().Shuffle().ToList();
                newGame.AddContracts(contracts);
                newGame.DrawContracts();
            }
        }

        public static void FinalizeSetup(this Game newGame)
        {
            newGame.ChooseVisitors();
            newGame.DrawInitialVisitors();
            newGame.SetupTickets();
            newGame.assignReputationTiles();
            newGame.PlayerOrder = newGame.Players.Shuffle().ToList();
        }
    }
}