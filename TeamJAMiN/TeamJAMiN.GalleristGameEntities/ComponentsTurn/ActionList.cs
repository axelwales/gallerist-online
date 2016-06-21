using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities.ComponentsTurn
{
    public abstract class ActionList<TList,TNode> : DataList<TList,TNode, GameTurn> where TList : ICollection<TNode>, new()
    {
        public int Count { get { return GetCount(); } }

        protected abstract int GetCount();

        public ActionList(GameTurn turn, string actionData)
        {
            Subject = turn;
            Data = actionData;
        }

        public ActionList(GameTurn turn, TList actionList)
        {
            Subject = turn;
            List = actionList;
        }

        public ActionList(GameTurn turn)
        {
            Subject = turn;
            Data = null;
            List = default(TList);
        }

        public ActionList()
        {

        }
    }
}
