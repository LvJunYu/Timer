// 攀墙、攀墙跳、加速跑、三段跳引导

using SoyEngine;
using UnityEngine;
namespace GameA.Game
{
	public class Character2Guide : Guide {
		UnitBase _sight;
		UnityNativeParticleItem _sightParticle;
		public override void OnStart () {
			MoveMainPlayer(PlayMode.Instance.MainPlayer.CenterPos + new IntVec2(160, 0));
			ShowText("Hi，我又来了，这次要介绍的是我们可爱又头绿的游酱～");
			BlockTouchInput();
		}

		public override void OnFinish () {
			if (_sight != null) {
				PlayMode.Instance.DestroyUnit(_sight);
			}
			if (_sightParticle != null) {
				_sightParticle.DestroySelf();
			}
			base.OnFinish();
		}

		public override void NextStep () {
			Debug.Log("NextStep");
			base.NextStep ();
			int idx = 1;
			if (_step == idx++) {	// 1
				ShowText("游酱是男孩子，运动能力比匠酱好很多，可以做出各种高难度的动作。");
			} else if (_step == idx++) {	// 2
				ShowText("现在我们的游酱正面对着一堵墙，我们先按住“跳跃按钮”跳起来。");
				Mask(EMaskType.JumpButton, () => {
				OpenTouchInput();
				});
			} else if (_step == idx++) {	// 3
				CloseMask();
				BlockTouchInput();
				GuideManager.Instance.LockJumpButton();
				PauseGame(22, ()=>{
					NextStep();
				});
			} else if (_step == idx++) {	// 4
				ShowText("贴住墙壁时，按住“右方向按钮”可以抓住墙慢慢滑下来。");
				Mask(EMaskType.RightButton, () => {
					OpenTouchInput();
				});
			// } else if (_step == idx++) {	// 5
			// 	Mask(EMaskType.RightButton, () => {
			// 		OpenTouchInput();
			// 	});
			} else if (_step == idx++) {	// 5
				// BlockTouchInput();
                GameRun.Instance.Continue();
				GuideManager.Instance.UnlockJumpButton();
				GuideManager.Instance.LockAxisInput("Horizontal", 1f);
				PauseGame(18, ()=>{
					NextStep();
				});
			} else if (_step == idx++) {	// 6
				CloseMask();
				ShowText("在抓墙往下滑的时候，按“跳跃按钮”，就能反向蹬墙跳出去了。");
				Mask(EMaskType.JumpButton, () => {
					OpenTouchInput();
				});
			} else if (_step == idx++) {	// 7
				GuideManager.Instance.UnlockAxisInput();
				CloseMask();
				BlockTouchInput();
                GameRun.Instance.Continue();
				ShowText("干的漂亮！");
			} else if (_step == idx++) {	// 8
				ShowText("反墙跳可以跳上一些平常上不去的地方，是不是有种武林高手的感觉？");
				
			} else if (_step == idx++) {	// 9
				ShowText("学会了反墙跳的神技，我们再来练习一下加速跑和三段跳吧～");
			} else if (_step == idx++) {	// 10
				HideText(()=>{
					QiePingIn(()=>{
					NextStep();
					});
				});
			} else if (_step == idx++) {	// 11
				MoveMainPlayer(new IntVec2(21440, 8320));
				// HideText(()=>{
				// 	QiePingOut(()=> {
				// 		NextStep();
				// 	});
				// });
				QiePingOut(()=> {
						NextStep();
					});
			} 
			
			
			else if (_step == idx++) {	// 12
				ShowText("加速跑能让游酱像风(了)一样奔跑，双击“方向按钮”并且按住不放，试着跑起来吧！");
				Mask(EMaskType.RightButton, () => {
					OpenTouchInput();
				});
			} else if (_step == idx++) {	// 13
				CloseMask();
				PauseGame(1, null);
				GuideManager.Instance.LockAxisInput("Horizontal", 1f);
				ShowText("很好，足够快了，按下“跳跃按钮”。");
				Mask(EMaskType.JumpButton, () => {
					OpenTouchInput();
				});
			} else if (_step == idx++) {	// 14
				GuideManager.Instance.LockJumpButton();
				CloseMask();
                GameRun.Instance.Continue();
				BlockTouchInput();
			} else if (_step == idx++) {	// 15
				GuideManager.Instance.UnlockJumpButton();
				ShowText("是不是比平时跳得高了很多？再按一次“跳跃按钮”试试～");
				PauseGame(2, null);
				Mask(EMaskType.JumpButton, () => {
					OpenTouchInput();
				});
			} else if (_step == idx++) {	// 16
				GuideManager.Instance.LockJumpButton();
				CloseMask();
                GameRun.Instance.Continue();
				BlockTouchInput();
			}
			else if (_step == idx++) {	// 17
				GuideManager.Instance.UnlockJumpButton();
				ShowText("哇，动作都不一样了呢，如果再按一次“跳跃按钮”呢？");
				PauseGame(2, null);
				Mask(EMaskType.JumpButton, () => {
					OpenTouchInput();
				});
			} else if (_step == idx++) {	// 18
				GuideManager.Instance.LockJumpButton();
				CloseMask();
                GameRun.Instance.Continue();
				BlockTouchInput();
			} else if (_step == idx++) {	// 19
				PauseGame(1, null); 
				BlockTouchInput();
				GuideManager.Instance.UnlockJumpButton();
				GuideManager.Instance.UnlockAxisInput();
				GuideManager.Instance.ResetScreenClickCD();
				ShowText("整个人都飞起来了，你要和太阳肩并肩了！");
			} else if (_step == idx++) {	// 20
				ShowText("真是好棒棒！所有的厉害招式你都学会了，还有什么能难倒你的？");
			} else if (_step == idx++) {	// 21
				ShowText("好的身手需要合适的地方才能施展，幸好我们有好多制作精良的关卡，快去挑战吧～");
			} else if (_step == idx++) {
                GameRun.Instance.Continue();
				Finish();
			}
		}

		public override void OnScreenClick () {
			int idx = 0;
			if (_step == idx++) {	// 0
				NextStep();
			} else if (_step == idx++) {	// 1
				NextStep();
			} else if (_step == idx++) {	// 2
				
			} else if (_step == idx++) {	// 3
				
			} else if (_step == idx++) {	// 4
				
			} else if (_step == idx++) {	// 5
				
			} else if (_step == idx++) {	// 6
				
			} else if (_step == idx++) {	// 7
				NextStep();
			} else if (_step == idx++) {	// 8
				NextStep();
			} else if (_step == idx++) {	// 9
				NextStep();
			} else if (_step == idx++) {	// 10
			} else if (_step == idx++) {	// 11
				
			} else if (_step == idx++) {	// 12
				
			} else if (_step == idx++) {	// 13
				
			} else if (_step == idx++) {	// 14
				
			} else if (_step == idx++) {	// 15
				
			} else if (_step == idx++) {	// 16
				
			} else if (_step == idx++) {	// 17
				
			} else if (_step == idx++) {	// 18
				
			} else if (_step == idx++) {	// 19
				NextStep();
			} else if (_step == idx++) {	// 20
				NextStep();
			} else if (_step == idx++) {	// 21
				NextStep();
			}
		}

		public override void OnInput (EGuideInputKey key) {
			if (_step == 2) {
				if (key == EGuideInputKey.JumpButton) {
					NextStep();
				}
			}
			if (_step == 4) {
				if (key == EGuideInputKey.RightButton) {
					NextStep();
				}
			}
			if (_step == 6) {
				if (key == EGuideInputKey.JumpButton) {
					NextStep();
				}
			}
			if (_step == 13) {
				if (key == EGuideInputKey.JumpButton) {
					NextStep();
				}
			}
			if (_step == 15) {
				if (key == EGuideInputKey.JumpButton) {
					NextStep();
				}
			}
			if (_step == 17) {
				if (key == EGuideInputKey.JumpButton) {
					NextStep();
				}
			}
		}

		public override void OnSpecialOperate (int operateCode) {
			// 跑步速度达到最大
			if (operateCode == 1) {
				if (_step == 12) {
					NextStep();
				}
			}
			if (operateCode == 2) {
				if (_step == 14) {
					NextStep();
				}
				if (_step == 16) {
					NextStep();
				}
				if (_step == 18) {
					NextStep();
				}
			}
		}

		public override void OnUpdate () {
			if (PlayMode.Instance.MainPlayer.CenterPos.x > 166666) {
				PlayMode.Instance.MainPlayer.CenterPos = new IntVec2(PlayMode.Instance.MainPlayer.CenterPos.x - 64000,
				PlayMode.Instance.MainPlayer.CenterPos.y);
				CameraManager.Instance.CurRollPos = new IntVec2(CameraManager.Instance.CurRollPos.x - 64000,
				CameraManager.Instance.CurRollPos.y);
			}
		}
	}
}
