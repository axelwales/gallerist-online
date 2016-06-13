using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class TemplateGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual HashSet<TemplateArtist> Artists { get; set; }
        public virtual HashSet<TemplateArt> Art { get; set; }
        public virtual HashSet<TemplateReputationTile> ReputationTiles { get; set; }
        public virtual HashSet<TemplateContract> Contracts { get; set; }
    }
}
