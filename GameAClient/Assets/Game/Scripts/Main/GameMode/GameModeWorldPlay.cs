using System;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA.Game
{
    public class GameModeWorldPlay : GameModePlay
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType)
        {
            if (!base.Init(project, param, startType))
            {
                return false;
            }
            _gameSituation = EGameSituation.World;
            return true;
		}

        public override void OnGameFailed()
		{
			byte[] record = GetRecord();
			float usedTime = Game.PlayMode.Instance.GameFailFrameCnt * Game.ConstDefineGM2D.FixedDeltaTime;

			SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
            _project.CommitPlayResult(
                false,
                usedTime,
                PlayMode.Instance.SceneState.CurScore,
                PlayMode.Instance.SceneState.GemGain,
                PlayMode.Instance.SceneState.MonsterKilled,
                PlayMode.Instance.SceneState.SecondLeft,
                PlayMode.Instance.MainUnit.Life,
                record,
                DeadMarkManager.Instance.GetDeadPosition(),
                ()=>{
			    LogHelper.Info("游戏成绩提交成功");
			    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
			    if (!PlayMode.Instance.SceneState.GameFailed) return;
			    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioFailed);
                GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Lose);
			}, (errCode)=>{
			    LogHelper.Info("游戏成绩提交失败");
			    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
			    if (!PlayMode.Instance.SceneState.GameFailed) return;
			    CommonTools.ShowPopupDialog("游戏成绩提交失败", null,
			        new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
                        CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnGameFailed));
			        }), 
			        new System.Collections.Generic.KeyValuePair<string, Action>("跳过", ()=>{
			            //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSuccess);
                        GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Lose);
			        }));
			});
		}

        public override void OnGameSuccess()
		{
			byte[] record = GetRecord();
			float usedTime = Game.PlayMode.Instance.GameSuccessFrameCnt * Game.ConstDefineGM2D.FixedDeltaTime;

            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
            _project.CommitPlayResult(
                true,
                usedTime,
                PlayMode.Instance.SceneState.CurScore,
                PlayMode.Instance.SceneState.GemGain,
                PlayMode.Instance.SceneState.MonsterKilled,
                PlayMode.Instance.SceneState.SecondLeft,
                PlayMode.Instance.MainUnit.Life,
                record,
                DeadMarkManager.Instance.GetDeadPosition(),
                ()=>{
                LogHelper.Info("游戏成绩提交成功");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                if (!PlayMode.Instance.SceneState.GameSucceed) return;
                GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSuccess);
                GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Win);
            }, (errCode)=>{
                LogHelper.Info("游戏成绩提交失败");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                if (!PlayMode.Instance.SceneState.GameFailed) return;
                CommonTools.ShowPopupDialog("游戏成绩提交失败", null,
                    new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
                        CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnGameFailed));
                    }), 
                    new System.Collections.Generic.KeyValuePair<string, Action>("跳过", ()=>{
                        //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSuccess);
                        GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Win);
                    }));
            });
		}

        private byte[] GetRecord()
        {
            GM2DRecordData recordData = new GM2DRecordData();
            recordData.Version = GM2DGame.Version;
            recordData.FrameCount = ConstDefineGM2D.FixedFrameCount;
            recordData.Data.AddRange(PlayMode.Instance.InputDatas);
            byte[] recordByte = GameMapDataSerializer.Instance.Serialize(recordData);
            byte[] record = null;
            if(recordByte == null)
            {
                LogHelper.Error("录像数据出错");
            }
            else
            {
                record = MatrixProjectTools.CompressLZMA(recordByte);
            }
            return record;
        }
    }
}
