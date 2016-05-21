using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    public class GameTurn
    {
        public int Id { get; set; }
        public int TurnNumber { get; set; }
        public GameTurnType Type { get; set; }
        public Game Game { get; set; }
        public Player CurrentPlayer { get; set; }
        public Player KickedOutPlayer { get; set; }

        public string CompletedActionData { get; set; }
        public string PendingActionData { get; set; }

        [NotMapped]
        public int CurrentActionOrderNumber
        {
            get
            {
                return CompletedActions.Count;
            }
        }

        [NotMapped]
        GameAction _currentAction { get; set; }
        [NotMapped]
        public GameAction CurrentAction
        {
            get
            {
                if (_currentAction == null && CompletedActions.Count > 0)
                {
                    _currentAction = CompletedActions[CompletedActions.Count - 1];
                }
                return _currentAction;
            }
            set
            {
                _currentAction = value;
            }
        }

        [NotMapped]
        List<GameAction> _pendingActions { get; set; }
        [NotMapped]
        public List<GameAction> PendingActions
        {
            get
            {
                if(_pendingActions == null)
                {
                    if(PendingActionData == null)
                    {
                        _pendingActions = new List<GameAction>();
                    }
                    else
                    {
                        _pendingActions = JsonConvert.DeserializeObject<List<GameAction>>(PendingActionData);
                        foreach(GameAction action in _pendingActions)
                        {
                            action.Turn = this;
                            if(action.ParentId != null)
                            {
                                 action.Parent = CompletedActions.FirstOrDefault(a => a.Order == action.ParentId);
                            }
                        }
                    }
                }
                return _pendingActions;
            }
            set
            {
                PendingActionData = JsonConvert.SerializeObject(value);
                _pendingActions = value;
            }
        }

        [NotMapped]
        List<GameAction> _completedActions { get; set; }
        [NotMapped]
        public List<GameAction> CompletedActions
        {
            get
            {
                if (_completedActions == null)
                {
                    if (CompletedActionData == null)
                    {
                        _completedActions = new List<GameAction>();
                    }
                    else
                    {
                        _completedActions = JsonConvert.DeserializeObject<List<GameAction>>(CompletedActionData);
                        foreach (GameAction action in _completedActions)
                        {
                            action.Turn = this;
                            if (action.ParentId != null)
                            {
                                action.Parent = CompletedActions.FirstOrDefault(a => a.Order == action.ParentId);
                            }
                        }
                    }
                }
                return _completedActions;
            }
            set
            {
                CompletedActionData = JsonConvert.SerializeObject(value);
                _completedActions = value;
            }
        }

    }
}
