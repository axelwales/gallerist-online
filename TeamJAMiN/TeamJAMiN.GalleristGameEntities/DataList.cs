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
    public abstract class DataList<TList,TNode, TSubject> where TList : ICollection<TNode>, new()
    {
        public string Data { get; set; }

        [NotMapped]
        public TSubject Subject { get; set; }

        [NotMapped]
        protected TList _list;

        [NotMapped]
        protected TList List { get { return GetDataList(); } set { SetDataList(value); } }

        [NotMapped]
        public TNode First { get { return GetFirst(); } }

        protected abstract TNode GetFirst();

        protected virtual void SetDataList(TList value)
        {
            Data = JsonConvert.SerializeObject(value);
            _list = value;
        }

        protected virtual TList GetDataList()
        {
            if (_list == null)
            {
                _list = JsonConvert.DeserializeObject<TList>(Data);
                if (_list == null)
                {
                    _list = new TList();
                }
                SetNavigationProperties(_list);
            }
            return _list;
        }

        protected virtual void SetNavigationProperties(TList list)
        {
        }

        public virtual void Add(TNode node)
        {
            List.Add(node);
            UpdateData();
        }

        public virtual void Remove(TNode value)
        {
            List.Remove(value);
            UpdateData();
        }

        public virtual TNode FirstOrDefault(Func<TNode,bool> predicate)
        {
            return List.FirstOrDefault(predicate);
        }

        protected virtual void UpdateData()
        {
            Data = JsonConvert.SerializeObject(List);
        }
    }
}
