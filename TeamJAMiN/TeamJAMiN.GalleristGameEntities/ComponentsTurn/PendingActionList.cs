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
        int _nextActionId = 0;
        public int NextActionId { get { return _nextActionId; } set { _nextActionId = value; } }

        protected override int GetCount()
        {
            return List.Count;
        }

        protected override List<GameAction> GetFirst()
        {
            return List.First.Value;
        }

        protected override void SetNavigationProperties(LinkedList<List<GameAction>> list)
        {
            foreach (List<GameAction> actionList in list)
            {
                foreach (GameAction action in actionList)
                {
                    action.Turn = Subject;
                    if (Subject != null)
                    {
                        action.Parent = Subject.GetActionById(action.ParentId);
                    }
                }
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
            foreach(GameAction action in newNode)
                action.Id = NextActionId++;
            List.AddFirst(newNode);
            UpdateData();
        }

        public void AddLast(GameAction action)
        {
            var newNode = new List<GameAction> { action };
            AddLast(newNode);
        }

        public void AddLast(List<GameAction> newNode)
        {
            foreach (var action in newNode)
                action.Id = NextActionId++;
            List.AddLast(newNode);
            UpdateData();
        }

        public void Remove(GameAction action)
        {
            var node = FirstOrDefaultList(a => a.State == action.State);
            if (node != null)
            {
                node.Remove(action);
                if (node.Count == 0)
                    Remove(node);
                UpdateData();
            }
        }

        public GameAction FirstOrDefault(Func<GameAction, bool> predicate)
        {
            GameAction result = null;
            var list = FirstOrDefaultList(predicate);
            if (list != null)
                result = list.First(predicate);
            return result;
        }

        public List<GameAction> FirstOrDefaultList(Func<GameAction, bool> predicate)
        {
            List<GameAction> result = null;
            foreach (var sublist in List)
            {
                var action = sublist.FirstOrDefault(predicate);
                if (action != null)
                {
                    result = sublist;
                    break;
                }
            }
            return result;
        }

        public PendingActionList() : base() { }
        public PendingActionList(GameTurn turn) : base(turn) { }
        public PendingActionList(GameTurn turn, string actionData) : base(turn, actionData) { }
        public PendingActionList(GameTurn turn, LinkedList<List<GameAction>> actionList) : base(turn, actionList) { }
    }
}
