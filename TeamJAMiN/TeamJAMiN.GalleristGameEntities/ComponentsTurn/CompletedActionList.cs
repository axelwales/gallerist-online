using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities.ComponentsTurn
{
    public class CompletedActionList : ActionList<List<GameAction>,GameAction>
    {
        public GameAction Last { get { return GetLast(); } }

        protected GameAction GetLast()
        {
            return List.Last();
        }

        protected override GameAction GetFirst()
        {
            return List.First();
        }

        protected override int GetCount()
        {
            return List.Count;
        }

        protected override void SetNavigationProperties(List<GameAction> list)
        {
            foreach (GameAction action in list)
            {
                action.Turn = Subject;
                if (Subject != null)
                {
                    action.Parent = Subject.GetActionById(action.ParentId);
                }
            }
        }

        public CompletedActionList() : base() { }
        public CompletedActionList(GameTurn turn) : base(turn) { }
        public CompletedActionList(GameTurn turn, string actionData) : base(turn, actionData) { }
        public CompletedActionList(GameTurn turn, List<GameAction> actionList) : base(turn, actionList) { }
    }
}
