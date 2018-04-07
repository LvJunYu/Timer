using SoyEngine;

namespace GameA
{
    public class USCtrlPasswordDoorSetting : USCtrlBase<USViewPasswordDoorSetting>
    {
        private USCtrlUnitPropertyEditButton[] _numCtrlArray;
        private UPCtrlUnitPropertyPasswordDoor _mainCtrl;

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
                else if (i == list.Length - 1)
                {
                    _numCtrlArray[0] = button;
                }
                else
                {
                    button.AddClickListener(() => _mainCtrl.ClearNums());
                }
            }

            for (int j = 0; j < _numCtrlArray.Length; j++)
            {
                var inx = j;
                _numCtrlArray[j].AddClickListener(() => _mainCtrl.SetNum(inx));
                _numCtrlArray[j].SetFgImage(UICtrlPasswordDoorInGame.GetSprite(j));
            }
        }

        public override void Open()
        {
            base.Open();
            _cachedView.SetActiveEx(true);
        }

        public override void Close()
        {
            base.Close();
            _cachedView.SetActiveEx(false);
        }

        public void SetMainCtrl(UPCtrlUnitPropertyPasswordDoor mainCtrl)
        {
            _mainCtrl = mainCtrl;
        }
    }
}