using SoyEngine;

namespace GameA
{
    public class UPCtrlWorldMulti : UPCtrlWorldPanelBase
    {
        public override void Close()
        {
            _cachedView.GridDataScrollers[(int) _menu].RefreshCurrent();
            base.Close();
        }

        public override void RequestData(bool append = false)
        {
            
        }
        
        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
        {
        }
    }
}