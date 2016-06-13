using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamJAMiN.GalleristComponentEntities
{
    [ComplexType]
    public abstract class DataList<TList,TNode> where TList : new()
    {
        public string Data { get; set; }
        
        protected TList _list;
        protected TList List { get { return GetActionList(); } set { SetActionList(value); } }

        public TNode First { get { return GetFirst(); } }

        protected abstract TNode GetFirst();

        protected virtual void SetActionList(TList value)
        {
            Data = JsonConvert.SerializeObject(value);
            _list = value;
        }

        protected virtual TList GetActionList()
        {
            if (_list == null)
            {
                if (Data == null)
                {
                    _list = new TList();
                }
                else
                {
                    _list = JsonConvert.DeserializeObject<TList>(Data);
                }
            }
            return _list;
        }

        public abstract void Add(TNode node);

        protected virtual void UpdateData()
        {
            Data = JsonConvert.SerializeObject(List);
        }

        protected abstract void AttachList();
    }
}
