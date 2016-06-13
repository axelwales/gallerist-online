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

        public override void Add(GameAction action)
        {
            List.Add(action);
            UpdateData();
        }

        protected override void UpdateData()
        {
            base.UpdateData();
            if (Turn != null)
                Turn.UpdateCompletedList(this);
        }

        protected override void AttachList()
        {
            if (Turn != null)
            {
                Turn.AddCompletedList(this);
            }
        }

        public CompletedActionList(GameTurn turn) : base(turn) { }
        public CompletedActionList(GameTurn turn, string actionData) : base(turn, actionData) { }
        public CompletedActionList(GameTurn turn, List<GameAction> actionList) : base(turn, actionList) { }
    }
}
