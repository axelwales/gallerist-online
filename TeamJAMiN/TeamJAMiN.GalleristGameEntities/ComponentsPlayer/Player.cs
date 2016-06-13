using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class Player
    {
        public Player()
        {
            Money = 10;
            Influence = 10;
            VipTickets = 0;
            InvestorTickets = 0;
            CollectorTickets = 0;
            Assistants = new HashSet<PlayerAssistant>();
            Tiles = new HashSet<GameReputationTile>();
            Contracts = new HashSet<GameContract>();
            Art = new HashSet<GameArt>();
        }
        public int Id { get; set; }

        public int GameId { get; set; }

        [MaxLength(128)]
        public string UserId { get; set; }
        public string UserName { get; set; }

        public bool IsHost { get; set; }
        public bool IsStartPlayer { get; set; }

        public Game Game { get; set; }
        public int Money { get; set; }
        public int Influence { get; set; }
        public HashSet<PlayerAssistant> Assistants { get; set; }
        public HashSet<GameReputationTile> Tiles { get; set; }
        public HashSet<GameContract> Contracts { get; set; }
        public HashSet<GameArt> Art { get; set; }
        public GameArtist Commission { get; set; }
        public PlayerColor Color { get; set; }
        public PlayerLocation GalleristLocation { get; set; }
        public int VipTickets { get; set; }
        public int InvestorTickets { get; set; }
        public int CollectorTickets { get; set; }
    }        
}
