using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities.ComponentsTurn
{
    public abstract class ActionList<TList,TNode> : DataList<TList,TNode> where TList : new()
    {
        protected GameTurn Turn { get; set; }

        public int Count { get { return GetCount(); } }

        protected abstract int GetCount();


        public ActionList(GameTurn turn, string actionData)
        {
            Turn = turn;
            Data = actionData;
            AttachList();
        }

        public ActionList(GameTurn turn, TList actionList)
        {
            Turn = turn;
            List = actionList;
            AttachList();
        }

        public ActionList(GameTurn turn)
        {
            Turn = turn;
            Data = null;
            List = default(TList);
            AttachList();
        }
    }
}
