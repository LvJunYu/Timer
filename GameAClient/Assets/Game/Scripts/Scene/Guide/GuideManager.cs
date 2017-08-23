using System;
using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA.Game
{
	public enum EGuideType {
		BasicRunJump,
		Character2,
	}
	public class GuideManager {
		private static GuideManager _instance;
		private Guide _currentGuide;

		private int _screenClickCold;

		private int _gamePauseTimer = -1;
		private Callback _gamePauseCB;
	
		private string _lockAxisName;
		private float _lockAxisValue;

		private bool _lockJumpButton = false;
		private bool _unlockedJumpButton = false;

		public bool IsGuiding {
			get {
				return _currentGuide != null;
			}
		}
		public bool IsLockJumpButton {
			get {
				return _lockJumpButton;
			}
		}
		public bool IsUnlockedJumpButton {
			get {
				if (_unlockedJumpButton) {
					_unlockedJumpButton = false;
					return true;
				}
				return false;
			}
		}

		public static GuideManager Instance {
			get { 
				if (_instance == null) {
					_instance = new GuideManager();
				}
				return _instance; 
			}
		}

		// public GuideManager () {
		// 	Messenger.AddListener (EMessengerType.GameFinishSuccessShowUI, OnGameSuccess);
		// }

		public void StartGuide (EGuideType guide) {
			switch (guide) {
				case EGuideType.BasicRunJump : {
					_currentGuide = new BasicRunJumpGuide ();
					_currentGuide.Start();
					break;
				}
				case EGuideType.Character2: {
					_currentGuide = new Character2Guide();
					_currentGuide.Start();
					break;
				}
				default: {
					break;
				}
			}
		}

		public void FinishCurrentGuide () {
			if (_currentGuide is BasicRunJumpGuide) {
				_currentGuide = null;
				PlayMode.Instance.SceneState.ForceSetTimeFinish ();
			} else if (_currentGuide is Character2Guide) {
				_currentGuide = null;
				PlayMode.Instance.SceneState.ForceSetTimeFinish ();
			}
			_lockAxisName = null;
			_lockJumpButton = false;
		}

		public void Update () {
			if (_currentGuide == null) return;
			if (_screenClickCold >= 0) _screenClickCold--;

			if (_gamePauseTimer > 0) {
				_gamePauseTimer--;
				if (_gamePauseTimer == 0) {
                    GameRun.Instance.Pause();
					if (_gamePauseCB != null) {
						_gamePauseCB();
					}
				}
			}

			_currentGuide.OnUpdate ();

			if (Input.anyKeyDown && _screenClickCold < 0) {
				_screenClickCold = 30;
				_currentGuide.OnScreenClick();
			}

			// if (PlayMode.Instance.MainPlayer != null) {
			// 	if (_currentGuide == null) return;
			// 	if (PlayMode.Instance.MainPlayer.Input.RightInput == 1) {
			// 		_currentGuide.OnInput(Guide.EGuideInputKey.RightButton);
			// 	} else if (PlayMode.Instance.MainPlayer.Input.RightInput == 2) {
			// 		_currentGuide.OnInput(Guide.EGuideInputKey.RightButtonDouble);
			// 	}
			// 	if (_currentGuide == null) return;
			// 	if (PlayMode.Instance.MainPlayer.Input.LeftInput == 1) {
			// 		_currentGuide.OnInput(Guide.EGuideInputKey.LeftButton);
			// 	} else if (PlayMode.Instance.MainPlayer.Input.LeftInput == 2) {
			// 		_currentGuide.OnInput(Guide.EGuideInputKey.LeftButtonDouble);
			// 	}
			// 	if (_currentGuide == null) return;
			// 	if (PlayMode.Instance.MainPlayer.Input.JumpInput) {
			// 		_currentGuide.OnInput(Guide.EGuideInputKey.JumpButton);
			// 	}
			// 	if (_currentGuide == null) return;
			// 	if (PlayMode.Instance.MainPlayer.Input.Skill1Input) {
			// 		_currentGuide.OnInput(Guide.EGuideInputKey.AttackButton);
			// 	}
			// }
			// 因为需要在游戏暂停时检测输入，所以只能直接用CrossPlatformInputManager的接口
			float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
			if (horizontal > 0.1f) {
				_currentGuide.OnInput(Guide.EGuideInputKey.RightButton);
			}
			if (horizontal < -0.1f) {
				_currentGuide.OnInput(Guide.EGuideInputKey.LeftButton);
			}
			if (CrossPlatformInputManager.GetButtonDown("Jump")) {
				_currentGuide.OnInput(Guide.EGuideInputKey.JumpButton);
			}
			if (CrossPlatformInputManager.GetButtonDown("Fire1")) {
				_currentGuide.OnInput(Guide.EGuideInputKey.AttackButton);
			}
			if (_lockAxisName != null) {
				CrossPlatformInputManager.SetAxis(_lockAxisName, _lockAxisValue);
			}
			// if (_lockJumpButton) {
			// 	CrossPlatformInputManager.SetButtonDown("Jump");
			// }
			
		}

		public void PauseGame (int delay, Callback cb) {
			if (delay > 0) {
				_gamePauseTimer = delay;
				_gamePauseCB = cb;
			}
		}

		public void LockAxisInput (string name, float value) {
			_lockAxisName = name;
			_lockAxisValue = value;
		}
		
		public void UnlockAxisInput () {
			CrossPlatformInputManager.SetAxis(_lockAxisName, 0);
			_lockAxisName = null;
		}

		public void LockJumpButton () {
			_lockJumpButton = true;
		}

		public void UnlockJumpButton () 
		{
			_lockJumpButton = false;
			_unlockedJumpButton = true;
			// CrossPlatformInputManager.SetButtonUp("Jump");
		}

		public void OnSpecialOperate (int operateCode) {
			if (_currentGuide != null)
			{
				_currentGuide.OnSpecialOperate(operateCode);
			}
		}

		public void OnGameSuccess () {
			if (_currentGuide != null) {
				_currentGuide.OnFinish();
				_currentGuide = null;
				_lockAxisName = null;
				_lockJumpButton = false;
			}
		}

		public void ResetScreenClickCD () {
			_screenClickCold = 30;
		}
	}

	public class Guide 
	{
		public enum EMaskType {
			None,
			RightButton,
			JumpButton,
			AttackButton,


		}

		public enum EGuideInputKey {
			LeftButton,
			LeftButtonDouble,
			RightButton,
			RightButtonDouble,
			JumpButton,
			AttackButton,
		}
		protected int _step;
		protected GuideUI _guideUI;
		// private Callback _animEndCB;


		public virtual void OnUpdate () {

		}
		public virtual void OnStart () {

		}
		public virtual void NextStep () {
			_step++;
		}

		public virtual void OnFinish () {
			// _guideUI.HideText(()=> {
				_guideUI.Close();
				_guideUI = null;
			// });
		}

		public virtual void OnScreenClick () {

		}

		public virtual void OnInput (EGuideInputKey key) {

		}

		public virtual void OnSpecialOperate (int operateCode) {
			// 跑步速度达到最大
			if (operateCode == 1) {

			}
		}

		protected void PauseGame (int delay, Callback cb) {
			GuideManager.Instance.PauseGame(delay, cb);
		}

		protected void MoveMainPlayer (IntVec2 pos) {
			if (PlayMode.Instance.MainPlayer != null) {
				PlayMode.Instance.MainPlayer.CenterDownPos = pos;
				PlayMode.Instance.MainPlayer.SetFacingDir(EMoveDirection.Right);
			}
		}

		protected void BlockTouchInput () {
			if (_guideUI != null) {
				_guideUI._screenBlock.raycastTarget = true;
			}
		}

		protected void OpenTouchInput () {
			if (_guideUI != null) {
				_guideUI._screenBlock.raycastTarget = false;
			}
		}

		protected void ShowText (string text) {
			if (_guideUI == null) {
				LoadGuideUI();
				_guideUI.SetText(text);
				return;
			}
			if (_guideUI == null) {
				LogHelper.Error("Open UICtrlGuideText failed");
				return;
			}

			_guideUI.ChangeText(text);
		}

		protected void HideText (Callback cb) {
			_guideUI.HideText(cb);
		}

		protected void ShowReward (Callback cb) {
			_guideUI.ShowReward(cb);
		}

		protected void HideReward () {
			_guideUI.HideReward();
		}
		protected void Mask (EMaskType maskType, Callback cb) {
			if (_guideUI == null) {
				LoadGuideUI();
			}
			_guideUI.SetMask(maskType, cb);
		}

		protected void CloseMask () {
			if (_guideUI == null) {
				LoadGuideUI();
			}
			_guideUI.SetMask(EMaskType.None);	
		}

		protected UnitBase CreateTrigger (IntVec2 pos, Callback cb) {
//			var guid = new IntVec3(pos.x, pos.y, GM2DTools.GetRuntimeCreatedUnitDepth());
//            var triggerObj = new UnitDesc(65400, guid, 0, Vector2.one);
//            var unit = PlayMode.Instance.CreateUnit(triggerObj);
//			if (unit != null) {
//				var triggerUnit = unit as Trigger;
//				if (triggerUnit != null) {
//					triggerUnit.SetCallback(cb);
//					return triggerUnit;
//				}
//			}
			return null;
		}

		protected void QiePingIn (Callback cb) {
			_guideUI.QiePingIn(cb);
		}

		protected void QiePingOut (Callback cb) {
			_guideUI.QiePingOut(cb);
		}

		public void Start () {
			OnStart ();
		}

		protected void Finish () {
			OnFinish();
			GuideManager.Instance.FinishCurrentGuide ();
		}

		private void LoadGuideUI () {
			if (_guideUI != null) return;
//#if UNITY_EDITOR
//            UnityEngine.Object obj = Resources.Load("GuideUI");
//            if (obj == null) {
////                obj = GameResourceManager.Instance.LoadMainAssetObject ("GuideUI");
//            }
//            var gameObj = CommonTools.InstantiateObject (obj);
//#else
//            var obj = GameResourceManager.Instance.LoadMainAssetObject("GuideUI");
//            var gameObj = CommonTools.InstantiateObject(obj) as GameObject;
//#endif
//			_guideUI = gameObj.GetComponent<GuideUI>();
		}
	}
}