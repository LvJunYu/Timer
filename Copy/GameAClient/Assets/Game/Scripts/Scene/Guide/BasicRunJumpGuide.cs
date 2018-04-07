// 基础跑、跳、吃引导
using SoyEngine;
using UnityEngine;
namespace GameA.Game
{
	public class BasicRunJumpGuide : Guide {
		UnitBase _sight;
		UnityNativeParticleItem _sightParticle;
		public override void OnStart () {
			ShowText("欢迎来到匠游世界！");
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
			base.NextStep ();
			int idx = 1;
			if (_step == idx++) {	// 1
				ShowText("我是新手引导员妞妞，下面由我给冒险者大人介绍这个世界的基本规则。");
			} else if (_step == idx++) {	// 2
				ShowText("屏幕下面的按钮可以控制我们的主角，试试按住“方向按钮”前进吧～");
				Mask(EMaskType.RightButton, () => {
				OpenTouchInput();
				});
			} else if (_step == idx++) {	// 3
				CloseMask();
				ShowText("就是这样，太棒了！");
				BlockTouchInput();
			} else if (_step == idx++) {	// 4
				ShowText("咦？前面好像有个好东西，走过去看看吧。");
				OpenTouchInput();
				_sight = CreateTrigger(new SoyEngine.IntVec2(18560, 8320),
				() => {
					NextStep();
					});
				_sightParticle = GameParticleManager.Instance.GetUnityNativeParticleItem("M1Sight", null);
				_sightParticle.Trans.position = GM2DTools.TileToWorld(new IntVec2(18880, 8320));
				_sightParticle.Play();
			} else if (_step == idx++) {	// 5
				HideText(null);
			} else if (_step == idx++) {	// 6
				if (_sightParticle != null) {
					_sightParticle.Stop();
				}
				BlockTouchInput();
				ShowReward(()=>{
					NextStep();
				});
			} else if (_step == idx++) {	// 7
			} else if (_step == idx++) {	// 8
				HideReward();
				ShowText("啊哈，是个宝箱！运气不错哦。");
			} else if (_step == idx++) {	// 9
				ShowText("接下来试试跳一下吧，点击右下角的“跳跃按钮”。");
				Mask(EMaskType.JumpButton, () => {
				OpenTouchInput();
				});
			} else if (_step == idx++) {	// 10
				CloseMask();
				ShowText("嗯，嗯，干得漂亮！");
				BlockTouchInput();
			} else if (_step == idx++) {	// 11
				ShowText("前面的台阶上也有一个宝箱，里面会有些什么呢？");
				OpenTouchInput();
				_sight = CreateTrigger(new SoyEngine.IntVec2(22400, 8960),
				() => {
					NextStep();
					});
				
				_sightParticle.Trans.position = GM2DTools.TileToWorld(new IntVec2(22720, 8960));
				_sightParticle.Play();
			} else if (_step == idx++) {	// 12
				HideText(null);
			} else if (_step == idx++) {	// 13
				if (_sightParticle != null) {
					_sightParticle.Stop();
				}
				BlockTouchInput();
				ShowReward(()=>{
					NextStep();
				});
			} else if (_step == idx++) {	// 14
			} else if (_step == idx++) {	// 15
				HideReward();
				ShowText("又捡到好多金币！虽然不知道有什么用，但肯定是好东西！");
			} else if (_step == idx++) {	// 16
				ShowText("基础练习完成了，下面我们去看看匠酱的特殊能力吧～");
			} else if (_step == idx++) {	// 17
				QiePingIn(()=>{
					NextStep();
				});
			} 
			
			
			
			else if (_step == idx++) {	// 18
				MoveMainPlayer(new IntVec2(36160, 8320));
				HideText(()=>{
					QiePingOut(()=> {
						NextStep();
					});
				});
				
			} else if (_step == idx++) {	// 19
				ShowText("别看我们的匠酱是个柔弱的女生，食量可不小（雾）");
			} else if (_step == idx++) {	// 20
				ShowText("面前是一个果冻，点击屏幕右下角的“吞下按钮”，尝尝是什么味道吧。");
				Mask(EMaskType.AttackButton, () => {
				OpenTouchInput();
				});
			} else if (_step == idx++) {	// 21
				CloseMask();
				ShowText("嗯...味道怪怪的。吃饱状态下的匠酱行动能力会降低，需要注意哦。");
				BlockTouchInput();
			} else if (_step == idx++) {	// 22
				ShowText("除了果冻，匠酱还能吃好多东西，在游戏中不断尝试吧～");
			} else if (_step == idx++) {	// 23
				ShowText("如果吃撑了，还能把肚子里的东西吐出来，再按一次“吞下按钮”试试。");
				Mask(EMaskType.AttackButton, () => {
				OpenTouchInput();
				});
			} else if (_step == idx++) {	// 24
				CloseMask();
				BlockTouchInput();
				ShowText("棒棒哒～");
			} else if (_step == idx++) {	// 25
				ShowText("冒险中可以利用这个动作来搬运各种东西呢，吃货的绝招～");
			} else if (_step == idx++) {	// 26
				ShowText("好了，现在你已经学会匠酱的所有招数了，在冒险中大展身手吧！");
			} else if (_step == idx++) {
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
				NextStep();
			} else if (_step == idx++) {	// 4
				NextStep();
			} else if (_step == idx++) {	// 5
				
			} else if (_step == idx++) {	// 6
				
			} else if (_step == idx++) {	// 7
				NextStep();
			} else if (_step == idx++) {	// 8
				NextStep();
			} else if (_step == idx++) {	// 9
				
			} else if (_step == idx++) {	// 10
				NextStep();
			} else if (_step == idx++) {	// 11
				NextStep();
			} else if (_step == idx++) {	// 12
				
			} else if (_step == idx++) {	// 13
				
			} else if (_step == idx++) {	// 14
				NextStep();
			} else if (_step == idx++) {	// 15
				NextStep();
			} else if (_step == idx++) {	// 16
				NextStep();
			} else if (_step == idx++) {	// 17
				
			} else if (_step == idx++) {	// 18
				
			} else if (_step == idx++) {	// 19
				NextStep();
			} else if (_step == idx++) {	// 20
				
			} else if (_step == idx++) {	// 21
				NextStep();
			} else if (_step == idx++) {	// 22
				NextStep();
			} else if (_step == idx++) {	// 23
				
			} else if (_step == idx++) {	// 24
				NextStep();
			} else if (_step == idx++) {	// 25
				NextStep();
			} else if (_step == idx++) {	// 26
				NextStep();
			}
		}

		public override void OnInput (EGuideInputKey key) {
			if (_step == 2) {
				if (key == EGuideInputKey.RightButton) {
					NextStep();
				}
			}
			if (_step == 9) {
				if (key == EGuideInputKey.JumpButton) {
					NextStep();
				}
			}
			if (_step == 20) {
				if (key == EGuideInputKey.AttackButton) {
					NextStep();
				}
			}
			if (_step == 23) {
				if (key == EGuideInputKey.AttackButton) {
					NextStep();
				}
			}
		}
	}
}
