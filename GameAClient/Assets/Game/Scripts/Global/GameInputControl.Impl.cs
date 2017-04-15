﻿using UnityEngine;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace GameA.Game
{
    public partial class GameInputControl : MonoBehaviour
    {

        public const float MoveInputButtonRatio = 1;

        private bool _pressLeft = false;
        private bool _pressRight = false;


        public void Reset ()
        {
            for (int i = 0; i < _jumpBtns.Length; i++) {
                if (_jumpBtns [i] != null) {
                    _jumpBtns [i].ResetState ();
                }
            }
            for (int i = 0; i < _fire1Btns.Length; i++) {
                if (_fire1Btns [i] != null) {
                    _fire1Btns [i].ResetState ();
                }
            }
            for (int i = 0; i < _fire2Btns.Length; i++) {
                if (_fire2Btns [i] != null) {
                    _fire2Btns [i].ResetState ();
                }
            }
            for (int i = 0; i < LeftButtons.Length; i++) {
                if (LeftButtons [i] != null) {
                    LeftButtons [i].ResetState ();
                }
            }
            for (int i = 0; i < RightGButtons.Length; i++) {
                if (RightGButtons [i] != null) {
                    RightGButtons [i].ResetState ();
                }
            }
        }

        public void ShowAttack1Btn ()
        {
            _fire1Btns [0].SetActiveEx (true);
        }

        private void OnGameRestart ()
        {
            SetM1YoyoFireBtnState (true);
        }

        private void OnMainPlayerRevive ()
        {

        }

        public void SetM1YoyoFireBtnState (bool eat)
        {
            if (eat) {
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2NumberSprites [0], out _fire1Btns [0].normalSprite);
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2NumberSprites [0], out _fire1Btns [0].pressedSprite);
                Sprite sprite = null;
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2NumberSprites [0], out sprite);
                _fire1Btns [0].GetComponent<UnityEngine.UI.Image> ().sprite = sprite;
            } else {
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2NumberSprites [1], out _fire1Btns [0].normalSprite);
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2NumberSprites [1], out _fire1Btns [0].pressedSprite);
                Sprite sprite = null;
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2NumberSprites [1], out sprite);
                _fire1Btns [0].GetComponent<UnityEngine.UI.Image> ().sprite = sprite;
            }
        }

        /// <summary>
        /// M2中设置Fire2的技能图标
        /// </summary>
        /// <returns>The m2 fire2 button sprite set.</returns>
        /// <param name="idx">第几套.</param>
        /// <param name="number">左下角数字.</param>
        public void SetM2Fire2BtnSpriteSet (int idx, int number)
        {
            if (idx >= M2FireBtn2NormalSprites.Length ||
                idx >= M2FireBtn2PressedSprites.Length ||
                idx >= M2FireBtn2DisableSprites.Length) {
                return;
            }
            if (number < 0 || number >= M2FireBtn2NumberSprites.Length) {
                return;
            }
            if (number > 0) {
                GameResourceManager.Instance.TryGetSpriteByName( M2FireBtn2NormalSprites [idx], out _fire2Btns [1].normalSprite);
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2PressedSprites [idx], out _fire2Btns [1].pressedSprite);
                Sprite sprite1 = null;
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2NormalSprites [idx], out sprite1);
                _fire2Btns [1].GetComponent<UnityEngine.UI.Image> ().sprite = sprite1;
            } else {
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2DisableSprites [idx], out _fire2Btns [1].normalSprite);
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2DisableSprites [idx], out _fire2Btns [1].pressedSprite);
                Sprite sprite2 = null;
                GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2DisableSprites [idx], out sprite2);
                _fire2Btns [1].GetComponent<UnityEngine.UI.Image> ().sprite = sprite2;
            }
            Sprite sprite = null;
            GameResourceManager.Instance.TryGetSpriteByName (M2FireBtn2NumberSprites [number], out sprite);
            M2Fire2BtnNumber.sprite = sprite;
        }

        void Awake()
        {
#if MOBILE_INPUT
            //_joystick.onMove.AddListener(OnJoystickMove);
            //_joystick.onMoveEnd.AddListener(OnJoystickMoveEnd);
            for (int i = 0; i < _jumpBtns.Length; i++) {
                if (_jumpBtns [i] != null)
                    _jumpBtns[i].onDown.AddListener (OnJumpBtnDown);
            }
            for (int i = 0; i < _jumpBtns.Length; i++) {
                if (_jumpBtns [i] != null)
                _jumpBtns[i].onUp.AddListener (OnJumpBtnUp);
            }
            for (int i = 0; i < _fire1Btns.Length; i++) {
                if (_fire1Btns [i] != null)
                _fire1Btns[i].onDown.AddListener (OnFire1BtnDown);
            }
            for (int i = 0; i < _fire1Btns.Length; i++) {
                if (_fire1Btns [i] != null)
                _fire1Btns[i].onUp.AddListener (OnFire1BtnUp);
            }
            for (int i = 0; i < _fire2Btns.Length; i++) {
                if (_fire2Btns [i] != null)
                _fire2Btns [i].onDown.AddListener (OnFire2BtnDown);
            }
            for (int i = 0; i < _fire2Btns.Length; i++) {
                if (_fire2Btns [i] != null)
                _fire2Btns [i].onUp.AddListener (OnFire2BtnUp);
            }
#endif

            for (int i = 0; i < LeftButtons.Length; i++) {
                if (LeftButtons [i] != null)
                LeftButtons[i].OnPress = OnLeftButtonDown;
            }
            for (int i = 0; i < LeftButtons.Length; i++) {
                if (LeftButtons [i] != null)
                LeftButtons[i].OnRelease = OnLeftButtonUp;
            }
            for (int i = 0; i < RightGButtons.Length; i++) {
                if (RightGButtons [i] != null)
                RightGButtons[i].OnPress = OnRightButtonDown;
            }
            for (int i = 0; i < RightGButtons.Length; i++) {
                if (RightGButtons [i] != null)
                RightGButtons[i].OnRelease = OnRightButtonUp;
            }
//            switch (GM2DGame.Instance.EMatrixType)
//            {
//                case EMatrixType.MT_JumpPlatform:
                    M1Input.SetActiveEx(true);
                    M2Input.SetActiveEx(false);
//                    break;
//            }
            Messenger<int, int>.AddListener (EMessengerType.OnSkillChanged, OnSkillChanged);
            Messenger<int, int>.AddListener (EMessengerType.OnAmmoChanged, OnAmmoChanged);
            Messenger.AddListener (EMessengerType.OnPlay, OnGameRestart);
            Messenger.AddListener (EMessengerType.OnMainPlayerRevive, OnMainPlayerRevive);
		}


        private void OnFire1BtnUp()
        {
            CrossPlatformInputManager.SetButtonUp(InputManager.TagSkill[0]);
        }

        private void OnFire2BtnUp ()
        {
            CrossPlatformInputManager.SetButtonUp (InputManager.TagSkill [1]);
        }

        private void OnFire1BtnDown()
        {
            CrossPlatformInputManager.SetButtonDown(InputManager.TagSkill[0]);
        }

        private void OnFire2BtnDown ()
        {
            CrossPlatformInputManager.SetButtonDown (InputManager.TagSkill [1]);
        }

        void OnDestroy()
        {
#if MOBILE_INPUT
            //_joystick.onMove.RemoveAllListeners();
            for (int i = 0; i < _fire1Btns.Length; i++) {
                if (_fire1Btns [i] != null)
                _fire1Btns[i].onDown.RemoveAllListeners ();
            }
            for (int i = 0; i < _fire1Btns.Length; i++) {
                if (_fire1Btns [i] != null)
                _fire1Btns[i].onUp.RemoveAllListeners ();
            }
            for (int i = 0; i < _fire2Btns.Length; i++) {
                if (_fire2Btns [i] != null)
                _fire2Btns [i].onDown.RemoveAllListeners ();
            }
            for (int i = 0; i < _fire2Btns.Length; i++) {
                if (_fire2Btns [i] != null)
                _fire2Btns [i].onUp.RemoveAllListeners ();
            }
            for (int i = 0; i < _jumpBtns.Length; i++) {
                if (_jumpBtns [i] != null)
                _jumpBtns[i].onDown.RemoveAllListeners ();
            }
            for (int i = 0; i < _jumpBtns.Length; i++) {
                if (_jumpBtns [i] != null)
                _jumpBtns[i].onUp.RemoveAllListeners ();
            }
            Messenger<int, int>.RemoveListener (EMessengerType.OnSkillChanged, OnSkillChanged);
            Messenger<int, int>.RemoveListener (EMessengerType.OnAmmoChanged, OnAmmoChanged);
            Messenger.RemoveListener (EMessengerType.OnPlay, OnGameRestart);
            Messenger.RemoveListener (EMessengerType.OnMainPlayerRevive, OnMainPlayerRevive);
#endif
        }

		private void OnJumpBtnDown()
        {
            CrossPlatformInputManager.SetButtonDown(InputManager.TagJump);
        }

        private void OnJumpBtnUp()
        {
            CrossPlatformInputManager.SetButtonUp(InputManager.TagJump);
        }

        private void OnJoystickMoveEnd()
        {
            CrossPlatformInputManager.SetAxis(InputManager.TagHorizontal, _joysticks[0].axisX.axisValue);
            CrossPlatformInputManager.SetAxis(InputManager.TagVertical, _joysticks[0].axisY.axisValue);
        }

        private void OnJoystickMove(Vector2 arg0)
        {
            CrossPlatformInputManager.SetAxis(InputManager.TagHorizontal, _joysticks [0].axisX.axisValue);
            CrossPlatformInputManager.SetAxis(InputManager.TagVertical, _joysticks [0].axisY.axisValue);
        }

	    private void OnLeftButtonDown()
	    {
	        _pressLeft = true;
            CrossPlatformInputManager.SetAxis(InputManager.TagHorizontal, GetHorizontalValue());
		}

		private void OnLeftButtonUp()
	    {
            _pressLeft = false;
            CrossPlatformInputManager.SetAxis(InputManager.TagHorizontal, GetHorizontalValue());
        }

        private void OnRightButtonDown()
        {
            _pressRight = true;
            CrossPlatformInputManager.SetAxis(InputManager.TagHorizontal, GetHorizontalValue());
        }

        private void OnRightButtonUp()
	    {
            _pressRight = false;
            CrossPlatformInputManager.SetAxis(InputManager.TagHorizontal, GetHorizontalValue());
        }

        private float GetHorizontalValue()
        {
            float l, r;
            l = _pressLeft ? -MoveInputButtonRatio : 0;
            r = _pressRight ? MoveInputButtonRatio : 0;
            return l + r;
        }

        private void OnSkillChanged (int skillId, int ammoNumber)
        {
            SetM2Fire2BtnSpriteSet (skillId, ammoNumber);
        }

        private void OnAmmoChanged (int skillId, int ammoNumber)
        {
            SetM2Fire2BtnSpriteSet (skillId, ammoNumber);
        }
    }


}