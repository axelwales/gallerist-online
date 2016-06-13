using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class TemplateContract
    {
        public int Id { get; set; }
        public ArtType Art { get; set; }
        public BonusType Bonus { get; set; }
    }
}
