using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlUnitPropertyPasswordDoor : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private const string NormalSpriteFormat = "img_{0}";
        private const string HeighlightSpriteFormat = "img_{0}_2";
        private USCtrlUnitPropertyEditButton[] _passwordDoorMenuList;
        private int _curIndex;
        private ushort[] _curPasswordArray;
        private int _count;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            var list = _cachedView.PasswordDoorDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _count = list.Length;
            _passwordDoorMenuList = new USCtrlUnitPropertyEditButton[_count];
            for (int i = 0; i < _count; i++)
            {
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _passwordDoorMenuList[i] = button;
            }

            _curPasswordArray = new ushort[_count];
        }

        public void RefreshView()
        {
            _curIndex = 0;
            var password = _mainCtrl.GetCurUnitExtra().CommonValue;
            for (int i = 0; i < _count; i++)
            {
                int num = password % (int) Mathf.Pow(10, _count - i) / (int) Mathf.Pow(10, _count - i - 1);
                RefreshNum(i, num, false);
            }
        }

        public void ClearNums()
        {
            _mainCtrl.GetCurUnitExtra().CommonValue = 0;
            RefreshView();
        }

        public void SetNum(int num)
        {
            if (_curIndex >= _count)
            {
                _curIndex = 0;
                for (int i = 0; i < _count; i++)
                {
                    RefreshNum(i, _curPasswordArray[i], false);
                }
            }
            RefreshNum(_curIndex, num, true);
            _curIndex++;
            _mainCtrl.GetCurUnitExtra().CommonValue = CalculatePassword();
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
                if (index < _count)
                {
                    _curPasswordArray[index] = (ushort) num;
                    _passwordDoorMenuList[index].SetFgImage(sprite);
                }
            }
        }

        private ushort CalculatePassword()
        {
            ushort password = 0;
            for (int i = 0; i < _count; i++)
            {
                password += (ushort) (_curPasswordArray[i] * Mathf.Pow(10, _count - i - 1));
            }

            return password;
        }
    }
}