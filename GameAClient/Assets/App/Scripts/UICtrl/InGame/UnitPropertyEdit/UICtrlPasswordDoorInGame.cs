using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlPasswordDoorInGame : UICtrlInGameBase<UIViewPasswordDoorInGame>
    {
        private const string BlankSpriteName = "img_blank";
        private const string NumSpriteFormat = "img_{0}";
        private const string NormalSpriteFormat = "img_{0}_3";
        private const string HeighlightSpriteFormat = "img_{0}_2";
        private const int ShowTime = 30;
        private USCtrlUnitPropertyEditButton[] _passwordDoorShowNumArray;
        private USCtrlUnitPropertyEditButton[] _numCtrlArray;
        private int _curIndex;
        private int[] _curValueArry;
        private int _count;
        private PasswordDoor _passwordDoor;
        private EState _curState;
        private bool _isShaking;
        private Vector2 _oriPos;
        private Vector2 _rockPos;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            var list = _cachedView.UpRtf.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _count = list.Length;
            _curValueArry = new int[_count];
            _passwordDoorShowNumArray = new USCtrlUnitPropertyEditButton[_count];
            for (int i = 0; i < _count; i++)
            {
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _passwordDoorShowNumArray[i] = button;
            }

            var numCtrl = _cachedView.DownRtf.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _numCtrlArray = new USCtrlUnitPropertyEditButton[10];
            for (int i = 0; i < numCtrl.Length; i++)
            {
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(numCtrl[i]);
                if (i < 9)
                {
                    _numCtrlArray[i + 1] = button;
                }
                else if (i == numCtrl.Length - 1)
                {
                    _numCtrlArray[0] = button;
                }
                else
                {
                    button.AddClickListener(ClearNums);
                }
            }

            for (int j = 0; j < _numCtrlArray.Length; j++)
            {
                var inx = j;
                _numCtrlArray[j].AddClickListener(() => SetNum(inx));
                _numCtrlArray[j].SetFgImage(GetSprite(j));
            }

            _oriPos = _cachedView.PannelRtf.anchoredPosition;
            _rockPos = _oriPos + Vector2.right * 10;
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _passwordDoor = parameter as PasswordDoor;
            if (_passwordDoor == null)
            {
                LogHelper.Error("UICtrlPasswordDoorInGame open fail, _passwordDoor == null");
                SocialGUIManager.Instance.CloseUI<UICtrlPasswordDoorInGame>();
                return;
            }

            _passwordDoor.OnUiOpen(true);
            SetState(EState.Normal);
            SocialApp.Instance.BanEsc = true;
        }

        protected override void OnClose()
        {
            SocialApp.Instance.BanEsc = false;
            _passwordDoor.OnUiOpen(false);
            base.OnClose();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_curState == EState.Correct && _passwordDoor.UiTimer == 0)
            {
                _passwordDoor.OnPasswordDoorOpen();
                SocialGUIManager.Instance.CloseUI<UICtrlPasswordDoorInGame>();
            }
            else if (_curState == EState.Wrong)
            {
                var timer = _passwordDoor.UiTimer;
                if (timer == 0)
                {
                    SetState(EState.Normal);
                }
                else
                {
                    _cachedView.PannelRtf.anchoredPosition =
                        Vector2.LerpUnclamped(_oriPos, _rockPos,
                            _cachedView.ShakeCure.Evaluate(1 - timer * ConstDefineGM2D.FixedDeltaTime));
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnCloseBtn();
            }

            if (_curState == EState.Normal)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
                {
                    SetNum(0);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                {
                    SetNum(1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                {
                    SetNum(2);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                {
                    SetNum(3);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                {
                    SetNum(4);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                {
                    SetNum(5);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                {
                    SetNum(6);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
                {
                    SetNum(7);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                {
                    SetNum(8);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
                {
                    SetNum(9);
                }
            }
        }

        private void SetNum(int num)
        {
            if (_curState != EState.Normal)
            {
                return;
            }

            if (SetShowNum(_curIndex, num, true))
            {
                _curIndex++;
                if (_curIndex == _count)
                {
                    TryOpen();
                }
            }
        }

        private void TryOpen()
        {
            if (_passwordDoor.HasCorrected)
            {
                Messenger<string>.Broadcast(EMessengerType.GameLog, "密码门已经被开启");
                SocialGUIManager.Instance.CloseUI<UICtrlPasswordDoorInGame>();
                return;
            }

            _passwordDoor.UiTimer = ShowTime;
            var curValue = CalculatePassword(_curValueArry);
            if (_passwordDoor.CheckOpen(curValue))
            {
                SetState(EState.Correct);
            }
            else
            {
                SetState(EState.Wrong);
            }
        }

        private void SetState(EState state)
        {
            _curState = state;
            _cachedView.NormalLightObj.SetActive(_curState == EState.Normal);
            _cachedView.WrongLightObj.SetActive(_curState == EState.Wrong);
            _cachedView.CorrectLightObj.SetActive(_curState == EState.Correct);
            _cachedView.PannelRtf.anchoredPosition = _oriPos;
            switch (state)
            {
                case EState.Normal:
                    ClearNums();
                    break;
                case EState.Correct:
                    break;
                case EState.Wrong:
                    break;
            }
        }

        private void ClearNums()
        {
            if (_curState != EState.Normal)
            {
                return;
            }

            _curIndex = 0;
            for (int i = 0; i < _count; i++)
            {
                SetShowNum(i, -1, false, JoyResManager.Instance.GetSprite(BlankSpriteName));
            }
        }

        private bool SetShowNum(int index, int num, bool hasSetting, Sprite sprite = null)
        {
            if (index < 0 || index >= _count)
            {
                LogHelper.Error("SetNum fail, index = {0}", index);
                return false;
            }

            if (num < -1 && num > 9)
            {
                LogHelper.Error("SetNum fail, num = {0}", num);
                return false;
            }

            if (num == -1)
            {
                _curValueArry[index] = 0;
                _passwordDoorShowNumArray[index].SetFgImage(sprite);
                return true;
            }

            _curValueArry[index] = num;
            _passwordDoorShowNumArray[index].SetFgImage(GetSprite(num, hasSetting));
            return true;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPasswordDoorInGame>();
        }

        public static Sprite GetSprite(int num)
        {
            return JoyResManager.Instance.GetSprite(string.Format(NumSpriteFormat, num));
        }

        public static Sprite GetSprite(int num, bool hasSetted)
        {
            string spriteName;
            if (hasSetted)
            {
                spriteName = string.Format(HeighlightSpriteFormat, num);
            }
            else
            {
                spriteName = string.Format(NormalSpriteFormat, num);
            }

            return JoyResManager.Instance.GetSprite(spriteName);
        }

        public static ushort CalculatePassword(int[] numArray)
        {
            var count = numArray.Length;
            ushort password = 0;
            for (int i = 0; i < count; i++)
            {
                password += (ushort) (numArray[i] * Mathf.Pow(10, count - i - 1));
            }

            return password;
        }

        public enum EState
        {
            Normal,
            Correct,
            Wrong
        }
    }
}