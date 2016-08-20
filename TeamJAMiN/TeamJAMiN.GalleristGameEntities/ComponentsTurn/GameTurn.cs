using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamJAMiN.GalleristComponentEntities.ComponentsTurn;

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
        public bool HasExecutiveAction { get; set; }

        [NotMapped]
        GameAction _currentAction { get; set; }

        [NotMapped]
        public GameAction CurrentAction
        {
            get
            {
                if (_currentAction == null && CompletedActions.Count > 0)
                {
                    _currentAction = CompletedActions.Last;  
                }
                return _currentAction;
            }
            set
            {
                _currentAction = value;
            }
        }

        [NotMapped]
        PendingActionList _pendingActions;

        public PendingActionList PendingActions
        {
            get
            {
                if(_pendingActions == null)
                    _pendingActions = new PendingActionList(this);
                if (_pendingActions.Subject == null)
                {
                    _pendingActions.Subject = this;
                }
                return _pendingActions;
            }
            set
            {
                _pendingActions = value;
            }
        }

        [NotMapped]
        CompletedActionList _completedActions { get; set; }

        public CompletedActionList CompletedActions
        {
            get
            {
                if (_completedActions == null)
                    _completedActions = new CompletedActionList(this);
                if (_completedActions.Subject == null)
                {
                    _completedActions.Subject = this;
                }
                return _completedActions;
            }
            set
            {
                _completedActions = value;
            }
        }

        public GameAction GetActionById(int? id)
        {
            GameAction action = null;
            if (id != null)
            {
                action = CompletedActions.FirstOrDefault(a => a.Id == id);
                if (action == null)
                    action = PendingActions.FirstOrDefault(a => a.Id == id);
            }
            return action;
        }
    }
}
