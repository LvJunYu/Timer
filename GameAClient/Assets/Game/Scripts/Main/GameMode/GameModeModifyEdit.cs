using SoyEngine;
using System;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class GameModeModifyEdit : GameModeEdit
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType, MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            _gameSituation = EGameSituation.Match;
            return true;
		}

        public override void OnGameStart()
        {
            base.OnGameStart();
            GameRun.Instance.ChangeState(ESceneState.Edit);
        }

        public override void OnGameFailed()
		{
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.EditorLose);
        }

        public override void OnGameSuccess()
		{
			//Debug.Log ("_______________________________1 " + p.ProjectId +" " + _project.PassFlag);
            if (false == _project.PassFlag)
			{
                _project.PassFlag = true;
				NeedSave = true;
				//Debug.Log ("_______________________________2 " + p.ProjectId +" " + _project.PassFlag);
			}
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.EditorLose);
        }

        public override void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
		{
			if (NeedSave)
			{
				// 保存改造关卡
				//SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "正在保存改造关卡");
                Save(() =>
				{
						//SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
						if (null != successCB)
					{
						successCB.Invoke();
					}
					SocialApp.Instance.ReturnToApp();
				},
					code =>
					{
							//SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
							if (null != failureCB)
						{
							failureCB.Invoke((int)code);
						}
						SocialApp.Instance.ReturnToApp();
							// todo error handle
							LogHelper.Error("SaveModifyProject failed");
					}
				);
				return;
			}
        }

        public override void Save(Action successCallback = null, Action<EProjectOperateResult> failedCallback = null)
		{
			byte[] mapDataBytes = MapManager.Instance.SaveMapData();
			byte[] compressedBytes = MatrixProjectTools.CompressLZMA(mapDataBytes);
			//            _project.SetBytesData (mapDataBytes);
			//Debug.Log ("_________________________3 " + _project.ProjectId + " " + _project.PassFlag);
			_project.SaveModifyProject(compressedBytes, successCallback, failedCallback);
        }

        public override void ChangeMode(EMode mode)
		{
			if (mode == _mode)
			{
				return;
			}
			_mode = mode;

			if (mode == EMode.EditTest)
			{
				SocialGUIManager.Instance.OpenUI<UICtrlEdit>();
				SocialGUIManager.Instance.GetUI<UICtrlEdit>().ChangeToEditTestMode();
				SocialGUIManager.Instance.OpenUI<UICtrlSceneState>();
				SocialGUIManager.Instance.CloseUI<UICtrlModifyEdit>();
				InputManager.Instance.ShowGameInput();
			}
			else if (mode == EMode.Edit)
			{
				SocialGUIManager.Instance.OpenUI<UICtrlEdit>().ChangeToModifyMode();
				SocialGUIManager.Instance.CloseUI<UICtrlSceneState>();
				SocialGUIManager.Instance.OpenUI<UICtrlModifyEdit>();
				InputManager.Instance.HideGameInput();
			}
		}
    }
}
