using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class GameAction : ICloneable
    {
        public int Id { get; set; }

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
                    ParentId = value.Id;
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
        public Dictionary<string,string> StateParams { get; set; }

        private string _location;
        public string Location
        {
            get { return _location; }
            set
            {
                StateParams["Location"] = value;
                _location = value;
            }
        }

        public bool IsExecutable { get; set; }
        public bool IsComplete { get; set; }

        public int Order { get; set; }

        public GameAction()
        {
            StateParams = new Dictionary<string, string>();
        }

        public object Clone()
        {
            var clone = new GameAction();
            clone.State = State;
            clone.Parent = Parent;
            foreach (KeyValuePair<string,string> param in StateParams)
            {
                clone.StateParams.Add(param.Key, param.Value);
            }
            return clone;
        }
    }
}