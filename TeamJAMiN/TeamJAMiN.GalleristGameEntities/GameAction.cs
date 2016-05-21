using Newtonsoft.Json;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class GameAction
    {
        public int? ParentId { get; set; }
        [JsonIgnore]
        GameAction _parent { get; set; }
        [JsonIgnore]
        public GameAction Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
                if(value != null)
                    ParentId = value.Order;
            }
        }

        public int? TurnId { get; set; }
        [JsonIgnore]
        GameTurn _turn { get; set; }
        [JsonIgnore]
        public GameTurn Turn
        {
            get
            {
                return _turn;
            }
            set
            {
                _turn = value;
                if (value != null)
                    TurnId = value.Id;
            }
        }

        public GameActionStatus Status { get; set; }
        public GameActionState State { get; set; }
        public string Location { get; set; }
        public bool IsExecutable { get; set; }
        public bool IsComplete { get; set; }
        public int Order { get; set; }
    }
}