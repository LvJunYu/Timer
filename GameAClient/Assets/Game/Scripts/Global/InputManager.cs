/********************************************************************
** Filename : InputManager
** Author : Dong
** Date : 2015/7/15 星期三 下午 3:16:28
** Summary : InputManager
***********************************************************************/

using SoyEngine;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace GameA.Game
{
    public class InputManager : MonoBehaviour
    {
        public readonly static string TagJump = "Jump";
        public readonly static string [] TagSkill = { "Fire1", "Fire2" };
        public readonly static string TagHorizontal = "Horizontal";
        public readonly static string TagVertical = "Vertical";
        public static InputManager Instance;

        public UICtrlGameInputControl GameInputControl;
		private bool _touchDown;
        private bool _keyJump;

        public bool IsTouchDown
        {
            get { return _touchDown; }
        }

        public void Awake()
        {
            Instance = this;
            gameObject.AddComponent<EasyTouch>();
            EasyTouch.On_TouchStart += EasyTouchOnOnTouchStart;
            EasyTouch.On_TouchDown += EasyTouchOnOnTouchDown;
            EasyTouch.On_TouchUp += EasyTouchOnOnTouchUp;
        }

        private void Start()
        {
            EasyTouch.SetEnable2DCollider(true);
            EasyTouch.AddCamera(CameraManager.Instance.RendererCamera);
        }

        private void OnDestroy()
        {
            Instance = null;
            EasyTouch.On_TouchStart -= EasyTouchOnOnTouchStart;
            EasyTouch.On_TouchDown -= EasyTouchOnOnTouchDown;
            EasyTouch.On_TouchUp -= EasyTouchOnOnTouchUp;
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
            if (null != GameInputControl) {
                GameInputControl.Show ();
                GameInputControl.ShowAttack1Btn ();
                GameInputControl.SetM1YoyoFireBtnState (true);
            }
        }

        public void HideGameInput()
        {
            if (null != GameInputControl) {
                GameInputControl.Hide ();
            }
        }
    }
}