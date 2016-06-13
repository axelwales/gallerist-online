using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class GameReputationTile
    {
        public GameReputationTile(TemplateReputationTile temp)
        {
            Influence = temp.Influence;
            Money = temp.Money;
            Scoring = temp.Scoring;
        }

        public GameReputationTile()
        {
        }

        public int Id { get; set; }
        public int Influence { get; set; }
        public int Money { get; set; }
        public ReputationTileScoring Scoring { get; set; }
        public GameReputationTileLocation Column {get; set;}
        public ArtType Row { get; set; }
    }
}
