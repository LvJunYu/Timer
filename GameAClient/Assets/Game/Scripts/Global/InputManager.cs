/********************************************************************
** Filename : InputManager
** Author : Dong
** Date : 2015/7/15 星期三 下午 3:16:28
** Summary : InputManager
***********************************************************************/

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    public class InputManager : IDisposable
    {
        public static InputManager _instance;

        public static readonly string TagJump = "Jump";
        public static readonly string[] TagSkill = {"Fire1", "Fire2"};
        public static readonly string TagHorizontal = "Horizontal";
        public static readonly string TagVertical = "Vertical";

        public UICtrlGameInputControl GameInputControl;
        private GameObject _easyTouchObject;
        private bool _keyJump;
        private bool _touchDown;

        public static InputManager Instance
        {
            get { return _instance ?? (_instance = new InputManager()); }
        }

        public bool IsTouchDown
        {
            get { return _touchDown; }
        }

        public void Dispose()
        {
            if (_easyTouchObject != null)
            {
                Object.Destroy(_easyTouchObject);
                _easyTouchObject = null;
            }
            if (_instance != null)
            {
                EasyTouch.On_TouchStart -= EasyTouchOnOnTouchStart;
                EasyTouch.On_TouchDown -= EasyTouchOnOnTouchDown;
                EasyTouch.On_TouchUp -= EasyTouchOnOnTouchUp;
                _instance = null;
            }
        }

        public void Init()
        {
            _easyTouchObject = new GameObject("EasyTouch");
            _easyTouchObject.AddComponent<EasyTouch>();
            EasyTouch.On_TouchStart += EasyTouchOnOnTouchStart;
            EasyTouch.On_TouchDown += EasyTouchOnOnTouchDown;
            EasyTouch.On_TouchUp += EasyTouchOnOnTouchUp;
            EasyTouch.SetEnable2DCollider(true);
            EasyTouch.AddCamera(CameraManager.Instance.RendererCamera);
        }

        private void EasyTouchOnOnTouchStart(Gesture gesture)
        {
        }

        private void EasyTouchOnOnTouchDown(Gesture gesture)
        {
            _touchDown = true;
        }

        private void EasyTouchOnOnTouchUp(Gesture gesture)
        {
            _touchDown = false;
        }

        public void ShowGameInput()
        {
            if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                HideGameInput();
                return;
            }
            if (null != GameInputControl)
            {
                GameInputControl.Show();
                GameInputControl.ShowAttack1Btn();
                GameInputControl.SetM1YoyoFireBtnState(true);
            }
        }

        public void HideGameInput()
        {
            if (null != GameInputControl)
            {
                GameInputControl.Hide();
            }
        }
    }
}