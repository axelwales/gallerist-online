using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class GameArtist
    {
        public GameArtist(TemplateArtist temp)
        {
            Category = temp.Category;
            ArtType = temp.ArtType;
            Fame = temp.Fame;
            InitialFame = temp.Fame;
            Promotion = temp.Promotion;
            StarLevels = temp.StarLevels;
            IsDiscovered = temp.IsDiscovered;
            AvailableArt = 2;
        }

        public GameArtist()
        {
            StarLevels = new int[] { };
        }

        public int Id { get; set; }
        public ArtistCategory Category { get; set; }
        public ArtType ArtType { get; set; }
        public BonusType DiscoverBonus { get; set; }
        public int Fame { get; set; }
        public int InitialFame { get; set; }
        public int Promotion { get; set; }
        public string StarLevelData { get; set; }
        public int[] StarLevels
        {
            get
            {
                return Array.ConvertAll(StarLevelData.Split(';'), int.Parse);
            }
            set
            {
                StarLevelData = String.Join(";", value.Select(s => s.ToString()).ToArray());
            }
        }
        public bool IsDiscovered { get; set; }
        public int AvailableArt { get; set; }
    }
}
