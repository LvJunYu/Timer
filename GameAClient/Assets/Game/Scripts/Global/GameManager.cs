/********************************************************************
** Filename : GameManager
** Author : Dong
** Date : 2015/3/23 16:00:02
** Summary : GameManager
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA
{
    public class GameManager : MonoBase
    {
        #region 常量与字段

        public static GameManager _instance;

        private GameBase _currentGame;
        private Type _gameType;

        private List<Project> _projectList = new List<Project>();
        private int _curProjectInx = 0;
        private EGameMode _gameMode;
        #endregion

        #region 属性

        public static GameManager Instance
        {
            get { return _instance ?? (_instance = new GameManager()); }
        }

        public GameBase CurrentGame
        {
            get { return _currentGame; }
        }

        public EGameMode GameMode
        {
            get { return _gameMode; }
            set { _gameMode = value; }
        }

        public int CurProjectIndex
        {
            get { return _curProjectInx; }
        }
        #endregion

        #region 方法

        public bool Init(Type gameType)
        {
            _gameType = gameType;
            return Init();
        }

        public override bool Init()
        {
            return true;
        }

        public override void Clear()
        {
        }

        public override void Update()
        {

        }

        /// <summary>
        ///     请求下载游戏
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool RequestLoadGame(GameBase game,Action<bool> success )
        {
            return true;
        }

        public bool RequestCreate(Project project)
        {
            _projectList.Clear();
            _projectList.Add(project);
            _curProjectInx = 0;
            return RequestStartGame(project, EStartType.Create);
        }

        public bool RequestEdit(Project project)
        {
            _projectList.Clear();
            _projectList.Add(project);
            _curProjectInx = 0;
            return RequestStartGame(project, EStartType.Edit);
        }

		public bool RequestModify(Project project)
		{
			_projectList.Clear();
			_projectList.Add(project);
			_curProjectInx = 0;
			return RequestStartGame(project, EStartType.ModifyEdit);
		}

        public bool RequestPlay(Project project)
        {
            _projectList.Clear();
            _projectList.Add(project);
            _curProjectInx = 0;
            return RequestStartGame(project, EStartType.Play);
        }

        public bool RequestPlayRecord(Project project, Record record)
        {
            _projectList.Clear();
            _projectList.Add(project);
            _curProjectInx = 0;
            return RequestStartGame(project, EStartType.PlayRecord, record);
        }

        public bool RequestPlay(List<Project> projectList, int inx = 0)
        {
            _projectList.Clear();
            _projectList.AddRange(projectList);
            _curProjectInx = inx-1;
            return PlayNext();
        }

        public bool PlayNext()
        {
            if (!HasNext())
            {
                return false;
            }
            Project p = _projectList[_curProjectInx+1];
            p.PrepareRes(() =>
            {
                if (_currentGame != null)
                {
                    RequestStopGame();
                    Messenger.Broadcast(EMessengerType.LoadEmptyScene);
                    CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() => {
                        System.GC.Collect();
                        _curProjectInx++;
                        RequestStartGame(p, EStartType.Play);
                    }));
                }
                else
                {
                    _curProjectInx++;
                    RequestStartGame(p, EStartType.Play);
                }
            }, () => {
                Messenger.Broadcast(EMessengerType.OnPrepareNextProjectResFailed);
            });
            return true;
        }

        public bool HasNext()
        {
            return _curProjectInx < _projectList.Count-1;
        }

        public Project GetNextProject()
        {
            if (!HasNext())
            {
                return null;
            }
            return _projectList[_curProjectInx+1];
        }

        /// <summary>
        /// 载入地图的时候以什么初衷载入
        /// </summary>
        public enum EStartType
        {
            None,
            Create,
            Edit,
            Play,
            PlayRecord,
			// 改造编辑
			ModifyEdit,
        }

        private bool RequestStartGame(Project project, EStartType eStartType, Record record = null)
        {
            //做成Component 为了切换Game时候的内存释放
            var go = new GameObject(_gameType.Name);
            var game = go.AddComponent(_gameType) as GameBase;
            if (game == null)
            {
                return false;
            }
            go.AddComponent<ResourceManager>();

            switch (eStartType)
            {
           	case EStartType.Create:
                if (!game.Create(project))
                {
                    LogHelper.Error("RequestStartGame failed, id:{0}, eStartType:{1}", project.ProjectId, eStartType);
                    return false;
                }
                break;
            case EStartType.Edit:
                if (!game.Edit(project))
                {
                    LogHelper.Error("RequestStartGame failed, id:{0}, eStartType:{1}", project.ProjectId, eStartType);
                    return false;
                }
                break;
			case EStartType.ModifyEdit:
				if (!game.ModifyEdit(project))
				{
					LogHelper.Error("RequestStartGame failed, id:{0}, eStartType:{1}", project.ProjectId, eStartType);
					return false;
				}
				break;
            case EStartType.Play:
                if (!game.Play(project))
                {
                    LogHelper.Error("RequestStartGame failed, id:{0}, eStartType:{1}", project.ProjectId, eStartType);
                    return false;
                }
                break;
            case EStartType.PlayRecord:
                if (!game.PlayRecord(project, record))
                {
                    LogHelper.Error("RequestStartGame failed, id:{0}, eStartType:{1}", project.ProjectId, eStartType);
                    return false;
                }
                break;
            }
            _currentGame = game;
            Messenger.Broadcast(EMessengerType.OnRequestStartGame);
            return true;
        }

        public bool RequestPauseGame()
        {
            if (_currentGame == null)
            {
                return false;
            }
            if (!_currentGame.Pause())
            {
                LogHelper.Error("RequestPauseGame failed, id:{0}", _currentGame.Project.ProjectId);
                return false;
            }
            return true;
        }

        /// <summary>
        ///     请求停止当前游戏
        /// </summary>
        /// <returns></returns>
        public bool RequestStopGame()
        {
            if (_currentGame == null)
            {
                return false;
            }
            Messenger.Broadcast(EMessengerType.OnGameStop);
            if (!_currentGame.Stop())
            {
                LogHelper.Error("RequestStopGame failed, id:{0}", _currentGame.Project.ProjectId);
                return false;
            }
            //清空释放内存
            Object.Destroy(_currentGame.gameObject);
			Messenger.Broadcast(EMessengerType.OnGameStopComplete);
			_currentGame = null;
            return true;
        }

        #endregion
    }

    public enum EGameMode
    {
        None,
        Normal,
        MatrixGuide,
        PlayRecord,
        NewGuide,
        OfficialProjectCollection,
    }
}