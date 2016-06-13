using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class TemplateReputationTile
    {
        public int Id { get; set; }
        public int Influence { get; set; }
        public int Money { get; set; }
        public ReputationTileScoring Scoring { get; set; }
    }
}
