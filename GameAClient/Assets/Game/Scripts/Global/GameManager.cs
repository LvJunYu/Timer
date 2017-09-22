/********************************************************************
** Filename : GameManager
** Author : Dong
** Date : 2015/3/23 16:00:02
** Summary : GameManager
***********************************************************************/

using System;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA
{
    public class GameManager : MonoBase
    {
        #region 常量与字段

        private static GameManager _instance;

        private GameBase _currentGame;
        private Type _gameType;

        private List<Project> _projectList = new List<Project>();
        private int _curProjectInx = 0;

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
            return RequestStartGame(project, EStartType.WorkshopCreate);
        }

        public bool RequestEdit(Project project)
        {
            _projectList.Clear();
            _projectList.Add(project);
            _curProjectInx = 0;
            return RequestStartGame(project, EStartType.WorkshopEdit);
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
            return RequestStartGame(project, EStartType.WorldPlay);
        }

        public bool RequestPlayAdvNormal (Project project, SituationAdventureParam param)
        {
            _projectList.Clear ();
            _projectList.Add (project);
            _curProjectInx = 0;
            return RequestStartGame (project, EStartType.AdventureNormalPlay, param);
        }

        public bool RequestPlayAdvBonus (Project project, SituationAdventureParam param)
        {
            _projectList.Clear ();
            _projectList.Add (project);
            _curProjectInx = 0;
            return RequestStartGame (project, EStartType.AdventureBonusPlay, param);
        }

        public bool RequestPlayAdvRecord(Project project, SituationAdventureParam param)
        {
            _projectList.Clear();
            _projectList.Add(project);
            _curProjectInx = 0;
            return RequestStartGame(project, EStartType.AdventureNormalPlayRecord, param);
        }

        public bool RequestPlayRecord(Project project, Record record)
        {
            _projectList.Clear();
            _projectList.Add(project);
            _curProjectInx = 0;
            return RequestStartGame(project, EStartType.WorldPlayRecord, record);
        }

        public bool RequestPlay(List<Project> projectList, int inx = 0)
        {
            _projectList.Clear();
            _projectList.AddRange(projectList);
            _curProjectInx = inx-1;
            return PlayNext();
        }


        public bool RequestPlayMultiCooperation(Project project)
        {
            _projectList.Clear();
            _projectList.Add(project);
            _curProjectInx = 0;
            return RequestStartGame(project, EStartType.MultiCooperationPlay, null);
        }

        public bool RequestPlayMultiBattle(Project project)
        {
            _projectList.Clear();
            _projectList.Add(project);
            _curProjectInx = 0;
            return RequestStartGame(project, EStartType.MultiBattlePlay, null);
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
                    //Messenger.Broadcast(EMessengerType.LoadEmptyScene);
                    CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() => {
                        System.GC.Collect();
                        _curProjectInx++;
                        RequestStartGame(p, EStartType.WorldPlay);
                    }));
                }
                else
                {
                    _curProjectInx++;
                    RequestStartGame(p, EStartType.WorldPlay);
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
            WorkshopCreate,
            WorkshopEdit,
            WorldPlay,
            WorldPlayRecord,
			// 改造编辑
			ModifyEdit,
            ChallengePlay,
            AdventureNormalPlay,
            AdventureNormalPlayRecord,
            AdventureBonusPlay,
            MultiCooperationPlay,
            MultiBattlePlay,
        }

        private bool RequestStartGame(Project project, EStartType eStartType, object param = null)
        {
            GameBase game;
            GameObject go = GameObject.Find (_gameType.Name);
            if (null != go) {
                game = go.GetComponent(_gameType) as GameBase;
            } else {
                go = new GameObject(_gameType.Name);
                game = go.AddComponent(_gameType) as GameBase;
            }
            //做成Component 为了切换Game时候的内存释放
            if (game == null)
            {
                return false;
            }
            JoyResManager.Instance.SetDefaultResScenary(EResScenary.Game);
            game.Play(project, param, eStartType);
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
}