using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class GameContract
    {
        public GameContract(TemplateContract temp)
        {
            Art = temp.Art;
            Bonus = temp.Bonus;
        }

        public GameContract()
        {
        }

        public int Id { get; set; }
        public ArtType Art { get; set; }
        public BonusType Bonus { get; set; }
        public int Order { get; set; }
        public GameContractLocation Location { get; set; }
    }
}
