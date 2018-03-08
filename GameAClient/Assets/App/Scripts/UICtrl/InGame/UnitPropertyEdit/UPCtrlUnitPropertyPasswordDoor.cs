using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlUnitPropertyPasswordDoor : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private const string NormalSpriteFormat = "img_{0}_2";
        private const string HeighlightSpriteFormat = "img_{0}_3";
        private USCtrlUnitPropertyEditButton[] _passwordDoorMenuList;
        private int _curIndex;
        private USCtrlPasswordDoorSetting _usCtrlPasswordDoorSetting;
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            var list = _cachedView.PasswordDoorDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _passwordDoorMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _passwordDoorMenuList[i] = button;
            }
            
            _usCtrlPasswordDoorSetting = new USCtrlPasswordDoorSetting();
            _usCtrlPasswordDoorSetting.Init(_cachedView.PasswordDoorSetting);
        }

        public override void Open()
        {
            base.Open();
            RefreshView();
            _usCtrlPasswordDoorSetting.Open();
            _curIndex = 0;
        }

        public override void Close()
        {
            _usCtrlPasswordDoorSetting.Close();
            base.Close();
        }

        public void RefreshView()
        {
            var password = _mainCtrl.GetCurUnitExtra().CommonValue;
            int count = _passwordDoorMenuList.Length;
            for (int i = 0; i < count; i++)
            {
                int num = password % (int) Mathf.Pow(10, count - i) / (int) Mathf.Pow(10, count - i - 1);
                RefreshNum(i, num, i < _curIndex);
            }
        }

        private void RefreshNum(int index, int num, bool hasSetted)
        {
            if (num < 0 || num > 9)
            {
                LogHelper.Error("RefreshNum fail, num = {0}", num);
            }

            string spriteName;
            if (hasSetted)
            {
                spriteName = string.Format(HeighlightSpriteFormat, num);
            }
            else
            {
                spriteName = string.Format(NormalSpriteFormat, num);
            }

            Sprite sprite;
            if (JoyResManager.Instance.TryGetSprite(spriteName, out sprite))
            {
                _passwordDoorMenuList[index].SetFgImage(sprite);
            }
        }
    }
}