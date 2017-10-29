using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorldPanelBase: UPCtrlBase<UICtrlWorld, UIViewWorld>, IOnChangeHandler<long>
    {
     

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
 
        }

        public override void Open()
        {
            base.Open();

        }

        public override void Close()
        {
        
            base.Close();
        }

        protected virtual void RefreshView()
        {
           
        }

        protected virtual void OnItemRefresh(IDataItemRenderer item, int inx)
        {
          
        }

        protected virtual void RequestData(bool append = false)
        {
        }

        public  virtual void OnChangeHandler(long val)
        {
        }

        public virtual void Set(EResScenary resScenary)
        {
          
        }

        public virtual  void SetMenu(UICtrlWorld.EMenu menu)
        {
          
        }

        public virtual void Clear()
        {
          
        }
    }
}