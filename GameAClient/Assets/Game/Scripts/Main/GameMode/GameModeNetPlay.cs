/********************************************************************
** Filename : GameModeNetPlay
** Author : Dong
** Date : 2017/6/23 星期五 下午 4:29:56
** Summary : GameModeNetPlay
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class GameModeNetPlay : GameModePlay
    {
        protected Dictionary<long, PlayerBase> _players = new Dictionary<long, PlayerBase>();
        protected List<Msg_RC_FrameData> _serverStartInputFrameList = new List<Msg_RC_FrameData>();
        protected Queue<Msg_RC_FrameData> _serverInputFrameQueue = new Queue<Msg_RC_FrameData>(128);
        protected int _preServerFrameCount;
        protected float _frameLeftTime;
        protected int _curServerFrame;
        protected EPhase _ePhase;
        protected int _bornSeed;
        protected int _recentServerFrame;
        
        public override bool IsMulti
        {
            get { return true; }
        }

        public override bool Stop()
        {
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
            Msg_CR_BattleResult msg = new Msg_CR_BattleResult();
            msg.Result = EBattleResult.EBR_Win;
            RoomManager.RoomClient.Write(msg);
            Loom.QueueOnMainThread(RoomManager.RoomClient.Disconnect, 3000);
        }

        public override void OnGameFailed()
        {
            Msg_CR_BattleResult msg = new Msg_CR_BattleResult();
            msg.Result = EBattleResult.EBR_Fail;
            RoomManager.RoomClient.Write(msg);
            Loom.QueueOnMainThread(RoomManager.RoomClient.Disconnect, 3000);
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
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
                            LocalPlayerInput localPlayerInput = PlayerManager.Instance.MainPlayer.Input as LocalPlayerInput;
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
                            SetPhase(EPhase.Normal);
                            isMain = true;
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
                if (frameDataList != null)
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
        
        public override bool IsPlayerCharacterAbilityAvailable(DynamicRigidbody unit,  ECharacterAbility eCharacterAbility)
        {
            //TODO 临时 应该使用Player的数据判断
            return true;
        }
        
        private void SetPhase(EPhase phase)
        {
            switch (phase)
            {
                case EPhase.None:
                    break;
                case EPhase.Simulation:
                    break;
                case EPhase.Pursue:
                    break;
                case EPhase.RequestStartBattle:
                    SendStartBattle();
                    break;
                case EPhase.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("phase", phase, null);
            }
            _ePhase = phase;
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
            if (_ePhase == EPhase.Normal
                || _ePhase == EPhase.Pursue
                || msg.FrameDatas[0].FrameInx >= _preServerFrameCount)
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

        internal void OnBattleClose(Msg_RC_BattleClose msg)
        {
        }

        internal void OnRoomInfo(Msg_RC_RoomInfo msg)
        {
            _bornSeed = msg.BornSeed;
            _preServerFrameCount = msg.CurrentRoomFrameCount;
            LogHelper.Info("RoomId: {0}", msg.RoomId);
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
        }
    }
}
