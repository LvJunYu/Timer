/********************************************************************
** Filename : GameModeNetPlay
** Author : Dong
** Date : 2017/6/23 星期五 下午 4:29:56
** Summary : GameModeNetPlay
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class GameModeNetPlay : GameModePlay
    {
        private static GameModeNetPlay _instance;
        
        protected Dictionary<long, PlayerBase> _players = new Dictionary<long, PlayerBase>();
        protected List<Msg_RC_FrameData> _serverStartInputFrameList = new List<Msg_RC_FrameData>();
        protected Queue<Msg_RC_FrameData> _serverInputFrameQueue = new Queue<Msg_RC_FrameData>(128);
        protected int _preServerFrameCount;
        protected float _frameLeftTime;
        protected int _curServerFrame;
        protected EPhase _ePhase;
        protected int _bornSeed;
        protected int _recentServerFrame;
        protected bool _loadingHasClosed;
        protected bool _localMyPlayerStarted;
        protected bool _serverMyPlayerStarted;
        private DebugFile _debugFile = DebugFile.Create("ServerData", "data.txt");
        private DebugFile _debugClientData = DebugFile.Create("ClientData", "clientData.txt");

        public override bool IsMulti
        {
            get { return true; }
        }

        public static DebugFile DebugClientData
        {
            get { return _instance._debugClientData; }
        }

        public GameModeNetPlay()
        {
            _instance = this;
        }

        public override bool Stop()
        {
            _debugFile.Close();
            _debugClientData.Close();
            if (!_loadingHasClosed)
            {
                base.OnGameStart();
            }
            SetPhase(EPhase.Close);
            RoomManager.RoomClient.Disconnect();
            if (!base.Stop())
            {
                return false;
            }
            _players.Clear();
            return true;
        }

        public override void OnGameSuccess()
        {
            if (!_loadingHasClosed)
            {
                base.OnGameStart();
            }
            SetPhase(EPhase.Succeed);
        }

        public override void OnGameFailed()
        {
            if (!_loadingHasClosed)
            {
                base.OnGameStart();
            }
            SetPhase(EPhase.Failed);
        }

        public override void OnGameStart()
        {
            _coroutineProxy.StopAllCoroutines();
            _coroutineProxy.StartCoroutine(GameFlow());
        }

        private IEnumerator GameFlow()
        {
            yield return null;
            _run = true;
            GameRun.Instance.Playing();
            _ePhase = EPhase.Simulation;
        }

        public override void Update()
        {
            if (_ePhase == EPhase.None)
            {
                return;
            }
            GameRun.Instance.Update();
            if (_ePhase == EPhase.Simulation)
            {
                var startTime = Time.realtimeSinceStartup;
                for (int i = _curServerFrame; i < _serverStartInputFrameList.Count; i++)
                {
                    while (!NeedApplyData())
                    {
                        ApplyFrameInputData(null);
                        ProcessLogic();
                    }
                    ApplyFrameData(_serverStartInputFrameList[i]);
                    ProcessLogic();
                    if (Time.realtimeSinceStartup - startTime > 1)
                    {
                        break;
                    }
                }
                if (_curServerFrame >= _preServerFrameCount)
                {
                    SetPhase(EPhase.Pursue);
                }
            }
            else if (_ePhase == EPhase.Pursue)
            {
                var startTime = Time.realtimeSinceStartup;
                while (_serverInputFrameQueue.Count > 0)
                {
                    while (!NeedApplyData())
                    {
                        ApplyFrameInputData(null);
                        ProcessLogic();
                    }
                    ApplyFrameData(_serverInputFrameQueue.Dequeue());
                    ProcessLogic();
                    if (Time.realtimeSinceStartup - startTime > 1)
                    {
                        break;
                    }
                }
                if (_serverInputFrameQueue.Count == 0)
                {
                    SetPhase(EPhase.RequestStartBattle);
                }
            }
            else if (_ePhase == EPhase.Close
                     || _ePhase == EPhase.Failed
                     || _ePhase == EPhase.Succeed)
            {
                
            }
            else
            {
                _frameLeftTime += Time.deltaTime;
                while (_serverInputFrameQueue.Count > 0)
                {
                    var needFrameTime = 25f / (_serverInputFrameQueue.Count + 23) * ConstDefineGM2D.FixedDeltaTime;
                    if (_frameLeftTime > needFrameTime)
                    {
                        _frameLeftTime -= needFrameTime;
                    }
                    else
                    {
                        break;
                    }
                    if (NeedApplyData())
                    {
                        ApplyFrameData(_serverInputFrameQueue.Dequeue());
                        if (null != PlayerManager.Instance.MainPlayer)
                        {
                            LocalPlayerInput localPlayerInput =
                                PlayerManager.Instance.MainPlayer.Input as LocalPlayerInput;
                            if (localPlayerInput != null)
                            {
                                localPlayerInput.ProcessCheckInput();
                                List<int> curInput = localPlayerInput.CurCheckInputChangeList;
                                if (curInput.Count > 0)
                                {
                                    SendInputDatas(GameRun.Instance.LogicFrameCnt, curInput);
                                }
                            }
                        }
                    }
                    else
                    {
                        ApplyFrameInputData(null);
                    }
                    ProcessLogic();
                }
                if (_serverInputFrameQueue.Count == 0)
                {
                    _frameLeftTime = 0;
                }
            }
        }

        private bool NeedApplyData()
        {
            return GameRun.Instance.LogicFrameCnt % 2 == 0;
        }

        private void ApplyFrameData(Msg_RC_FrameData frameData)
        {
            _debugFile.Write(_curServerFrame + " " + GameRun.Instance.LogicFrameCnt + "");
            _debugFile.WriteLine(JsonConvert.SerializeObject(frameData));
            PlayerManager pm = PlayerManager.Instance;
            for (int i = 0; i < frameData.CommandDatas.Count; i++)
            {
                var commandData = frameData.CommandDatas[i];
                ERoomUserCommand cmd = (ERoomUserCommand) commandData.Command;
                switch (cmd)
                {
                    case ERoomUserCommand.ERUC_None:
                        break;
                    case ERoomUserCommand.ERUC_JoinRoom:
                        pm.JoinRoom(commandData.UserData);
                        break;
                    case ERoomUserCommand.ERUC_StartBattle:
                        var roomUser = pm.GetRoomUserByInx(commandData.UserRoomInx);
                        bool isMain = false;
                        if (roomUser.Guid == LocalUser.Instance.UserGuid)
                        {
                            isMain = true;
                            _serverMyPlayerStarted = true;
                            if (_localMyPlayerStarted)
                            {
                                SetPhase(EPhase.Normal);
                            }
                        }
                        PlayMode.Instance.AddPlayer(_bornSeed, isMain, commandData.UserRoomInx);
                        break;
                }
            }
            ApplyFrameInputData(frameData.UserInputDatas);
            _curServerFrame++;
        }

        private void ApplyFrameInputData(List<Msg_RC_UserInputData> frameDataList)
        {
            PlayerManager pm = PlayerManager.Instance;
            for (int i = 0; i < pm.PlayerList.Count; i++)
            {
                PlayerBase playerBase = pm.PlayerList[i];
                if (playerBase == null)
                {
                    continue;
                }
                Msg_RC_UserInputData userInputData = null;
                if (frameDataList != null && frameDataList.Count > 0)
                {
                    userInputData = frameDataList.Find(m => m.UserRoomInx == i);
                }
                if (userInputData == null)
                {
                    playerBase.Input.ApplyInputData(null);
                }
                else
                {
                    playerBase.Input.ApplyInputData(userInputData.InputDatas);
                }
            }
        }

        private void ProcessLogic()
        {
            GameRun.Instance.UpdateLogic(ConstDefineGM2D.FixedDeltaTime);
        }

        public override bool IsPlayerCharacterAbilityAvailable(DynamicRigidbody unit,
            ECharacterAbility eCharacterAbility)
        {
            //TODO 临时 应该使用Player的数据判断
            return true;
        }

        private void SetPhase(EPhase phase)
        {
            _ePhase = phase;
            switch (_ePhase)
            {
                case EPhase.None:
                    break;
                case EPhase.Simulation:
                    break;
                case EPhase.Pursue:
                    break;
                case EPhase.RequestStartBattle:
                    if (_serverMyPlayerStarted)
                    {
                        SetPhase(EPhase.Normal);
                    }
                    else
                    {
                        SendStartBattle();
                    }
                    _localMyPlayerStarted = true;
                    break;
                case EPhase.Normal:
                    base.OnGameStart();
                    _loadingHasClosed = true;
                    break;
                case EPhase.Succeed:
                {
                    Msg_CR_BattleResult msg = new Msg_CR_BattleResult();
                    msg.Result = EBattleResult.EBR_Win;
                    RoomManager.RoomClient.Write(msg);
                    Loom.QueueOnMainThread(()=>
                    {
                        if (RoomManager.RoomClient.IsConnected())
                        {
                            RoomManager.RoomClient.Disconnect();
                        }
                    }, 1000);
                }
                    break;
                case EPhase.Failed:
                {
                    Msg_CR_BattleResult msg = new Msg_CR_BattleResult();
                    msg.Result = EBattleResult.EBR_Fail;
                    RoomManager.RoomClient.Write(msg);
                    Loom.QueueOnMainThread(()=>
                    {
                        if (RoomManager.RoomClient.IsConnected())
                        {
                            RoomManager.RoomClient.Disconnect();
                        }
                    }, 1000);
                }
                    break;
                case EPhase.Close:
                break;
                default:
                    throw new ArgumentOutOfRangeException("phase", phase, null);
            }
        }

        #region Send

        private void SendToServer(object msg)
        {
            var roomClient = RoomManager.RoomClient;
            if (roomClient != null && roomClient.IsConnected())
            {
                roomClient.Write(msg);
            }
        }

        public void SendStartBattle()
        {
            var msg = new Msg_CR_StartBattle();
            msg.Flag = 1;
            SendToServer(msg);
        }

        public void SendInputDatas(int frameInx, List<int> datas)
        {
            var msg = new Msg_CR_FrameInputDataArray();
            Msg_CR_FrameInputData msgFrame = new Msg_CR_FrameInputData();
            msgFrame.FrameInx = frameInx;
            msgFrame.InputDatas.AddRange(datas);
            msg.InputFrames.Add(msgFrame);
            SendToServer(msg);
        }

        public void SendExitBattle()
        {
//            var msg = new Msg_CR_UserExitBattle();
//            msg.Flag = 1;
//            SendToServer(msg);
        }

        public void SendBattleResult(EBattleResult eBattleResult)
        {
            var msg = new Msg_CR_BattleResult();
            msg.Result = eBattleResult;
            SendToServer(msg);
        }

        #endregion

        #region Receive

        internal void OnInputDatas(Msg_RC_FrameDataArray msg)
        {
            if (msg.FrameDatas[0].FrameInx >= _preServerFrameCount)
            {
                for (int i = 0; i < msg.FrameDatas.Count; i++)
                {
                    _serverInputFrameQueue.Enqueue(msg.FrameDatas[i]);
                }
                if (msg.FrameDatas.Count > 0)
                {
                    _recentServerFrame = msg.FrameDatas[msg.FrameDatas.Count - 1].FrameInx;
                }
            }
            else
            {
                _serverStartInputFrameList.AddRange(msg.FrameDatas);
            }
        }

        internal void OnRoomClose(ERoomCloseCode code)
        {

            switch (_ePhase)
            {
                case EPhase.None:
                case EPhase.Simulation:
                case EPhase.Pursue:
                case EPhase.RequestStartBattle:
                {
                    switch (code)
                    {
                        case ERoomCloseCode.ERCC_None:
                            break;
                        case ERoomCloseCode.ERCC_BattleEnd:
                            SocialGUIManager.ShowPopupDialog("战斗已结束，正在退出");
                            break;
                        case ERoomCloseCode.ERCC_RoomNotExist:
                            SocialGUIManager.ShowPopupDialog("战斗已结束，正在退出");
                            break;
                        case ERoomCloseCode.ERCC_WaitTimeout:
                            SocialGUIManager.ShowPopupDialog("房间等待玩家超时，正在退出");
                            break;
                        case ERoomCloseCode.ERCC_BattleTimeout:
                            break;
                        case ERoomCloseCode.ERCC_NoActiveUser:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("code", code, null);
                    }
                    SocialApp.Instance.ReturnToApp();
                }
                    break;
                case EPhase.Normal:
                case EPhase.Succeed:
                case EPhase.Failed:
                {
                    switch (code)
                    {
                        case ERoomCloseCode.ERCC_None:
                            break;
                        case ERoomCloseCode.ERCC_BattleEnd:
                            break;
                        case ERoomCloseCode.ERCC_RoomNotExist:
                            SocialGUIManager.ShowPopupDialog("战斗已结束，正在退出");
                            break;
                        case ERoomCloseCode.ERCC_WaitTimeout:
                            SocialGUIManager.ShowPopupDialog("房间等待玩家超时，正在退出");
                            break;
                        case ERoomCloseCode.ERCC_BattleTimeout:
                            return;
                        case ERoomCloseCode.ERCC_NoActiveUser:
                            return;
                        default:
                            throw new ArgumentOutOfRangeException("code", code, null);
                    }
                    SocialApp.Instance.ReturnToApp();
                }
                    break;
                case EPhase.Close:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal void OnRoomInfo(Msg_RC_RoomInfo msg)
        {
            _bornSeed = msg.BornSeed;
            _preServerFrameCount = msg.CurrentRoomFrameCount;
            LogHelper.Info("RoomId: {0}", msg.RoomId);
        }

        internal void OnDisconnected()
        {
            if (_ePhase == EPhase.Failed
                || _ePhase == EPhase.Close
                || _ePhase == EPhase.Succeed)
            {
                return;
            }
            SocialGUIManager.ShowPopupDialog("联机服务异常，正在退出");
            SocialApp.Instance.ReturnToApp();
        }

        #endregion

        protected enum EPhase
        {
            None,

            /// <summary>
            /// 模拟进入房间前的数据指令
            /// </summary>
            Simulation,

            /// <summary>
            /// 追赶进入房间后的指令
            /// </summary>
            Pursue,

            /// <summary>
            /// 请求开始
            /// </summary>
            RequestStartBattle,

            /// <summary>
            /// 正常运行
            /// </summary>
            Normal,
            Succeed,
            Failed,
            Close,
        }
    }
}