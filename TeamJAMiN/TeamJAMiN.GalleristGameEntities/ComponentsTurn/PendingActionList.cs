using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities.ComponentsTurn
{
    public class PendingActionList : ActionList<LinkedList<List<GameAction>>,List<GameAction>>
    {
        protected override int GetCount()
        {
            return List.Count;
        }

        protected override List<GameAction> GetFirst()
        {
            return List.First.Value;
        }

        protected override LinkedList<List<GameAction>> GetActionList()
        {
            _list = base.GetActionList();
            if (_list.Any())
            {
                foreach (List<GameAction> actionList in _list)
                {
                    foreach (GameAction action in actionList)
                        SetReferenceVariables(action);
                }
            }
            return _list;
        }

        private void SetReferenceVariables(GameAction action)
        {
            action.Turn = Turn;
            if (action.ParentId != null && Turn != null)
            {
                action.Parent = Turn.GetActionById(action.ParentId);
            }
        }

        protected override void UpdateData()
        {
            base.UpdateData();
            if (Turn != null)
                Turn.UpdatePendingList(this);
        }

        protected override void AttachList()
        {
            if (Turn != null)
            {
                Turn.AddPendingList(this);
            }
        }

        public List<GameAction> Pop()
        {
            var first = First;
            List.RemoveFirst();
            UpdateData();
            return first;
        }

        public void Add(GameAction action)
        {
            var newNode = new List<GameAction> { action };
            Add(newNode);
        }

        public override void Add(List<GameAction> newNode)
        {
            List.AddFirst(newNode);
            UpdateData();
        }

        public void Remove(List<GameAction> value)
        {
            List.Remove(value);
            UpdateData();
        }

        public PendingActionList(GameTurn turn) : base(turn) { }
        public PendingActionList(GameTurn turn, string actionData) : base(turn, actionData) { }
        public PendingActionList(GameTurn turn, LinkedList<List<GameAction>> actionList) : base(turn, actionList) { }
    }
}
