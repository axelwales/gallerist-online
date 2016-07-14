using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Models.ComponentViewModels
{
    public class ReputationTileViewModel
    {
        public ReputationTileViewModel(GameReputationTile tile)
        {
            Tile = tile;
            ScoringHtmlString = ReputationTileScoringHTML[tile.Scoring];
            ActionLocation = tile.Row.ToString() + ':' + tile.Column.ToString();
        }

        public GameReputationTile Tile { get; private set; }
        public string ActionLocation { get; private set; }
        public GameActionState State = GameActionState.Reputation;
        public string ScoringHtmlString { get; private set; }

        public static Dictionary<ReputationTileScoring, string> ReputationTileScoringHTML = new Dictionary<ReputationTileScoring, string>
        {
            { ReputationTileScoring.aquired, @"<div id=""aquired-scoring"" class=""row no-gutter"">
                                                    <div class=""col-xs-6"">
                                                        <div id=""aquired-scoring-sold"" class=""sell-icon aquired-scoring-icon""></div>
                                                    </div>
                                                    <div class=""col-xs-6"">
                                                        <div id=""aquired-scoring-exhibited"" class=""exhibit-icon aquired-scoring-icon""></div>
                                                    </div>
                                                </div>" },
            { ReputationTileScoring.artType, @"<div id=""art-type-scoring"" class=""row no-gutter"">
                                                    <div class=""col-xs-12"">
                                                        <div id=""art-type-scoring-sculpture"" class=""sculpture-icon art-type-scoring-art""></div>
                                                    </div>
                                                    <div class=""col-xs-6"">
                                                        <div id=""art-type-scoring-painting"" class=""painting-icon art-type-scoring-art""></div>
                                                    </div>
                                                    <div class=""col-xs-6"">
                                                        <div id=""art-type-scoring-photo"" class=""photo-icon art-type-scoring-art""></div>
                                                    </div>
                                                    <div class=""col-xs-12"">
                                                        <div id=""art-type-scoring-digital"" class=""digital-icon art-type-scoring-art""></div>
                                                    </div>
                                                </div>" },
            { ReputationTileScoring.assistant, "<div class=\"reputation-tile-scoring assistant-icon\"></div>" },
            { ReputationTileScoring.auction, "<div class=\"reputation-tile-scoring auction-icon\"></div>" },
            { ReputationTileScoring.collector, "<div class=\"reputation-tile-scoring visitor-collector\"></div>" },
            { ReputationTileScoring.digital, "<div class=\"reputation-tile-scoring digital-icon\"></div>" },
            { ReputationTileScoring.exhibiting, "<div class=\"reputation-tile-scoring exhibit-icon\"></div>" },
            { ReputationTileScoring.fame, "<div class=\"reputation-tile-scoring star-reputation-tile\"></div>" },
            { ReputationTileScoring.investor, "<div class=\"reputation-tile-scoring visitor-investor\"></div>" },
            { ReputationTileScoring.masterpiece, "<div id=\"masterpiece-scoring\" class=\"reputation-tile-scoring star-celebrity\"></div>" },
            { ReputationTileScoring.painting, "<div class=\"reputation-tile-scoring painting-icon\"></div>" },
            { ReputationTileScoring.photo, "<div class=\"reputation-tile-scoring photo-icon\"></div>" },
            { ReputationTileScoring.promotion, "<div id=\"promotion-scoring\" class=\"promotion-tile promotion-4\">4</div>" },
            { ReputationTileScoring.reputationTile, "<div class=\"reputation-tile-scoring reputation-icon\"></div>" },
            { ReputationTileScoring.sculpture, "<div class=\"reputation-tile-scoring sculpture-icon\"></div>" },
            { ReputationTileScoring.sold, "<div class=\"reputation-tile-scoring sell-icon\"></div>" },
            { ReputationTileScoring.threeInflunce, "<div class=\"reputation-tile-scoring influence-3\"></div>" },
            { ReputationTileScoring.vip, "<div class=\"reputation-tile-scoring visitor-vip\"></div>" },
            { ReputationTileScoring.visitor, "<div class=\"reputation-tile-scoring visitor-any\"></div>" },
            { ReputationTileScoring.visitorSet, @"<div id=""visitor-set-scoring"" class=""row no-gutter"">
                                                    <div class=""col-xs-12"">
                                                        <div id=""visitor-set-collector"" class=""visitor-collector visitor-set-scoring-visitor""></div>
                                                    </div>
                                                    <div class=""col-xs-6"">
                                                        <div id=""visitor-set-vip"" class=""visitor-vip visitor-set-scoring-visitor""></div>
                                                    </div>
                                                    <div class=""col-xs-6"">
                                                        <div id=""visitor-set-investor"" class=""visitor-investor visitor-set-scoring-visitor""></div>
                                                    </div>
                                                </div>" },
        };
    }
}