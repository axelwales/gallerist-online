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

        public string CompletedActionData { get; set; }
        public string PendingActionData { get; set; }

        public int NextPendingActionId { get; set; }

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

        [NotMapped]
        public PendingActionList PendingActions
        {
            get
            {
                if(_pendingActions == null)
                    _pendingActions = new PendingActionList(this, PendingActionData);
                return _pendingActions;
            }
            set
            {
                PendingActionData = value.Data;
                _pendingActions = value;
            }
        }

        [NotMapped]
        CompletedActionList _completedActions { get; set; }

        [NotMapped]
        public CompletedActionList CompletedActions
        {
            get
            {
                if (_completedActions == null)
                    _completedActions = new CompletedActionList(this, CompletedActionData);
                return _completedActions;
            }
            set
            {
                CompletedActionData = value.Data;
                _completedActions = value;
            }
        }

        public GameAction GetActionById(int? id)
        {
            if (id != null) ;
            return null;
        }

        internal void AddPendingList(PendingActionList actionList)
        {
            UpdatePendingList(actionList);
        }

        public void UpdatePendingList(PendingActionList actionList)
        {
            PendingActionData = actionList.Data;
        }

        internal void AddCompletedList(CompletedActionList actionList)
        {
            UpdateCompletedList(actionList);
        }

        public void UpdateCompletedList(CompletedActionList actionList)
        {
            CompletedActionData = actionList.Data;
        }
    }
}
