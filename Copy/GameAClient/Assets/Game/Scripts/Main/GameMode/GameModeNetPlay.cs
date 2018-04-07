﻿/********************************************************************
** Filename : GameModeNetPlay
** Author : Dong
** Date : 2017/6/23 星期五 下午 4:29:56
** Summary : GameModeNetPlay
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class GameModeNetPlay : GameModePlay
    {
        protected List<Msg_RC_FrameData> _serverStartInputFrameList = new List<Msg_RC_FrameData>();
        protected Queue<Msg_RC_FrameData> _serverInputFrameQueue = new Queue<Msg_RC_FrameData>(128);
        protected int _preServerFrameCount;
        protected float _frameLeftTime;
        protected int _curServerFrame;
        protected EPhase _ePhase;
        protected int _recentServerFrame;
        protected bool _loadingHasClosed;
        protected int _maxUserCount;
        private DebugFile _debugFile = DebugFile.Create("ServerData", "data.txt");
        private RoomInfo _roomInfo;
        private EGamePhase _curGamePhase;
        private bool _loadComplete;

        public override bool IsMulti
        {
            get { return true; }
        }

        public RoomInfo RoomInfo
        {
            get { return _roomInfo; }
        }

        public EGamePhase CurGamePhase
        {
            get { return _curGamePhase; }
        }

        public override bool Init(Project project, object param, GameManager.EStartType startType,
            MonoBehaviour corountineProxy)
        {
            _maxUserCount = project.NetData.PlayerCount;
            return base.Init(project, param, startType, corountineProxy);
        }

        public override bool Stop()
        {
            _debugFile.Close();
            TryCloseLoading(true);
            SetPhase(EPhase.Close);
            RoomManager.RoomClient.Disconnect();
            AppData.Instance.ChatData.ClearRoomChat();
            if (!base.Stop())
            {
                return false;
            }

            return true;
        }

        public override void OnGameSuccess()
        {
            TryCloseLoading();

            SetPhase(EPhase.Succeed);
        }

        public override void OnGameFailed()
        {
            TryCloseLoading();

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
            _loadComplete = true;
            //TODO 创建玩家
            GameRun.Instance.Playing();
            if (_curGamePhase == EGamePhase.Battle)
            {
                SetPhase(EPhase.Simulation);
            }
            else
            {
                SendLoadComplete();
                SetPhase(EPhase.Normal);
            }
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
                    SendLoadComplete();
                    SetPhase(EPhase.Normal);
                }
            }
            else if (_ePhase == EPhase.Close)
            {
            }
            else if (_ePhase == EPhase.Failed || _ePhase == EPhase.Succeed)
            {
                GameRun.Instance.UpdateSkeletonAnimation();
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

            if (_curGamePhase == EGamePhase.CountDown)
            {
                GameRun.Instance.UpdateSkeletonAnimation();
            }
        }

        private bool NeedApplyData()
        {
            return GameRun.Instance.LogicFrameCnt % 2 == 0;
        }

        private void ApplyFrameData(Msg_RC_FrameData frameData)
        {
            if (_debugFile.Enable)
            {
                _debugFile.Write(_curServerFrame + " " + GameRun.Instance.LogicFrameCnt + "");
                _debugFile.WriteLine(JsonConvert.SerializeObject(frameData));
            }

            ApplyFrameInputData(frameData.UserInputDatas);
            _curServerFrame++;
        }

        private void ApplyFrameInputData(List<Msg_RC_UserInputData> frameDataList)
        {
            PlayerManager pm = PlayerManager.Instance;
            for (int i = 0; i < _maxUserCount; i++)
            {
                PlayerBase playerBase = pm.GetPlayerByRoomIndex(i);
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

        protected override void InitUI()
        {
            base.InitUI();
            if (_curGamePhase <= EGamePhase.Wait)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlMultiRoom>(_roomInfo);
            }

            SocialGUIManager.Instance.OpenUI<UICtrlChatInGame>();
        }

        public override void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
        {
            base.QuitGame(successCB, failureCB, forceQuitWhenFailed);
            RoomManager.Instance.SendExitRoom();
        }

        private void SetGamePhase(EGamePhase gamePhase)
        {
            switch (_curGamePhase)
            {
                case EGamePhase.None:
                    break;
                case EGamePhase.Wait:
                    break;
                case EGamePhase.CountDown:
                    break;
                case EGamePhase.Battle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("gamePhase", gamePhase, null);
            }

            _curGamePhase = gamePhase;
            switch (_curGamePhase)
            {
                case EGamePhase.None:
                    break;
                case EGamePhase.Wait:
                    break;
                case EGamePhase.CountDown:
                    _serverInputFrameQueue.Clear();
                    _serverStartInputFrameList.Clear();
                    _recentServerFrame = 0;
                    _curServerFrame = 0;
                    if (_loadComplete)
                    {
                        GameRun.Instance.RePlay();
                        GameRun.Instance.Playing();
                        base.OnGameStart();
                    }

                    SocialGUIManager.Instance.GetUI<UICtrlSceneState>().ShowCountDown(true);
                    SocialGUIManager.Instance.CloseUI<UICtrlMultiRoom>();
                    break;
                case EGamePhase.Battle:
                    SetPhase(EPhase.Normal);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("gamePhase", gamePhase, null);
            }
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
                case EPhase.Normal:
                    TryCloseLoading();
                    break;
                case EPhase.Succeed:
                {
                    Msg_CR_BattleResult msg = new Msg_CR_BattleResult();
                    msg.Result = EBattleResult.EBR_Win;
                    RoomManager.RoomClient.Write(msg);
                    Loom.QueueOnMainThread(() =>
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
                    Loom.QueueOnMainThread(() =>
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

        public void OnRoomChangePos(Msg_RC_ChangePos msg)
        {
            if (_roomInfo == null)
            {
                return;
            }

            _roomInfo.OnRoomChangePos(msg);
            Messenger.Broadcast(EMessengerType.OnRoomPlayerInfoChanged);
        }

        public void OnRoomPlayerReadyChanged(Msg_RC_UserReadyInfo msg)
        {
            if (_roomInfo == null)
            {
                return;
            }

            _roomInfo.OnRoomPlayerReadyChanged(msg);
            Messenger.Broadcast(EMessengerType.OnRoomPlayerInfoChanged);
        }

        public void OnRoomUserEnter(Msg_RC_RoomUserInfo msg)
        {
            if (_roomInfo == null)
            {
                return;
            }

            _roomInfo.OnRoomUserEnter(msg);
            Messenger.Broadcast(EMessengerType.OnRoomPlayerInfoChanged);
        }

        public void OnUserKick(Msg_RC_Kick msg)
        {
            if (msg.UserGuid == LocalUser.Instance.UserGuid)
            {
                _ePhase = EPhase.Close;
                SocialGUIManager.ShowPopupDialog("您已被房主踢出游戏", null,
                    new KeyValuePair<string, Action>("确定", () => { SocialApp.Instance.ReturnToApp(); }));
            }
            else
            {
                if (_roomInfo == null)
                {
                    return;
                }

                _roomInfo.OnUserLeave(msg.UserGuid);
                Messenger.Broadcast(EMessengerType.OnRoomPlayerInfoChanged);
            }
        }

        public void OnUserExit(Msg_RC_UserExit msg)
        {
            if (_roomInfo == null) return;
            _roomInfo.OnUserExit(msg);
            Messenger.Broadcast(EMessengerType.OnRoomPlayerInfoChanged);
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

        public void SendLoadComplete()
        {
            var msg = new Msg_CR_LoadComplete();
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
            if (_curGamePhase == EGamePhase.CountDown)
            {
                if (msg.FrameDatas[0].FrameInx != 0)
                {
                    return;
                }

                SetGamePhase(EGamePhase.Battle);
            }

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
            LogHelper.Info("OnRoomClose: {0}", code.ToString());
            switch (code)
            {
                case ERoomCloseCode.ERCC_None:
                    break;
                case ERoomCloseCode.ERCC_BattleTimeout:
                case ERoomCloseCode.ERCC_BattleEnd:
                case ERoomCloseCode.ERCC_NoActiveUser:
                    if (_ePhase == EPhase.Failed
                        || _ePhase == EPhase.Succeed)
                    {
                        return;
                    }

                    SocialGUIManager.ShowPopupDialog("战斗已结束，正在退出");
                    break;
                case ERoomCloseCode.ERCC_RoomNotExist:
                    SocialGUIManager.ShowPopupDialog("战斗已结束，正在退出");
                    break;
                case ERoomCloseCode.ERCC_WaitTimeout:
                    SocialGUIManager.ShowPopupDialog("房间等待玩家超时，正在退出");
                    break;
                case ERoomCloseCode.ERCC_PrepareTimeout:
                    SocialGUIManager.ShowPopupDialog("房间准备超时，正在退出");
                    break;
                default:
                    SocialGUIManager.ShowPopupDialog("房间超时，正在退出");
                    break;
            }

            _ePhase = EPhase.Close;
            SocialApp.Instance.ReturnToApp();
        }

        internal void OnRoomInfo(Msg_RC_RoomInfo msg)
        {
            _roomInfo = new RoomInfo(msg);
            PlayerManager.Instance.SetRoomInfo(_roomInfo);
            _curGamePhase = (EGamePhase) msg.Phase;
            if (_curGamePhase == EGamePhase.Battle)
            {
                _preServerFrameCount = msg.PhaseParam;
            }

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

        protected void TryCloseLoading(bool isStop = false)
        {
            if (!_loadingHasClosed)
            {
                if (isStop)
                {
                    Messenger.Broadcast(EMessengerType.OnLoadingErrorCloseUI);
                }
                else
                {
                    base.OnGameStart();
                }

                _loadingHasClosed = true;
            }
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
            /// 正常运行
            /// </summary>
            Normal,
            Succeed,
            Failed,
            Close,
        }

        public enum EGamePhase
        {
            None,
            Wait,
            CountDown,
            Battle,
        }

        public void OnRoomOpen()
        {
            SetGamePhase(EGamePhase.CountDown);
        }
    }
}