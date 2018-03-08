using SoyEngine;

namespace GameA
{
    public class USCtrlPasswordDoorSetting : USCtrlBase<USViewPasswordDoorSetting>
    {
        private USCtrlUnitPropertyEditButton[] _numCtrlArray;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            var list = _cachedView.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _numCtrlArray = new USCtrlUnitPropertyEditButton[10];
            for (int i = 0; i < list.Length; i++)
            {
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                if (i < 9)
                {
                    _numCtrlArray[i + 1] = button;
                }
                else if ( i == list.Length-1)
                {
                    _numCtrlArray[0] = button;
                }
                else
                {
                    
                }
            }
        }
    }
}