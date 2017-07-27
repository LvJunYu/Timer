/********************************************************************
** Filename : GameModeNetPlay
** Author : Dong
** Date : 2017/6/23 星期五 下午 4:29:56
** Summary : GameModeNetPlay
***********************************************************************/

using System.Collections;
using System.Collections.Generic;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class GameModeNetPlay : GameModePlay
    {
        protected Dictionary<long, PlayerBase> _players = new Dictionary<long, PlayerBase>();
        protected bool _startBattleMsg;
        protected Queue<Msg_RC_FrameInputData> _serverInputFrameQueue = new Queue<Msg_RC_FrameInputData>(128);
        protected float _frameLeftTime;
        
        public override bool Stop()
        {
            if (!base.Stop())
            {
                return false;
            }
            _startBattleMsg = false;
            _players.Clear();
            return true;
        }

        public override void OnGameSuccess()
        {
        }

        public override void OnGameFailed()
        {
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
            SendEnterBattle();
            _coroutineProxy.StopAllCoroutines();
            _coroutineProxy.StartCoroutine(GameFlow());
        }

        private IEnumerator GameFlow()
        {
            yield return new WaitUntil(() => _startBattleMsg);
            UICtrlCountDown uictrlCountDown = SocialGUIManager.Instance.OpenUI<UICtrlCountDown>();
            yield return new WaitUntil(() => uictrlCountDown.ShowComplete);
            GameRun.Instance.Playing();
        }

        public override void Update()
        {
            GameRun.Instance.Update();
            _frameLeftTime += Time.deltaTime;
            while (_serverInputFrameQueue.Count > 0)
            {
                var needFrameTime = 50f / (_serverInputFrameQueue.Count + 46) * ConstDefineGM2D.FixedDeltaTime;
                if (_frameLeftTime > needFrameTime)
                {
                    _frameLeftTime -= needFrameTime;
                }
                else
                {
                    break;
                }
                LocalPlayerInput localPlayerInput = PlayerManager.Instance.MainPlayer.PlayerInput as LocalPlayerInput;
                if (localPlayerInput != null)
                {
                    localPlayerInput.ProcessCheckInput();
                    List<int> curInput = localPlayerInput.CurCheckInputChangeList;
                    if (curInput.Count > 0)
                    {
                        SendInputDatas(GameRun.Instance.LogicFrameCnt, curInput);
                    }
                }
                
                Msg_RC_FrameInputData frameInputData = _serverInputFrameQueue.Dequeue();
                PlayerManager pm = PlayerManager.Instance;
                for (int i = 0; i < pm.PlayerList.Count; i++)
                {
                    PlayerBase playerBase = pm.PlayerList[i];
                    Msg_RC_UserInputData userInputData =
                        frameInputData.UserInputDatas.Find(m => m.UserRoomInx == i);
                    if (userInputData == null)
                    {
                        playerBase.PlayerInput.ApplyInputData(null);
                    }
                    else
                    {
                        playerBase.PlayerInput.ApplyInputData(userInputData.InputDatas);
                    }
                }
                GameRun.Instance.UpdateLogic(ConstDefineGM2D.FixedDeltaTime);
            }
            if (_serverInputFrameQueue.Count == 0)
            {
                _frameLeftTime = 0;
            }
        }

        public override bool IsPlayerCharacterAbilityAvailable(PlayerBase player, ECharacterAbility eCharacterAbility)
        {
            //TODO 临时 应该使用Player的数据判断
            return true;
        }

        #region Send

        private void SendToServer(object msg)
        {
            var roomClient = RoomManager.RoomClient;
            if (roomClient != null && roomClient.IsConnnected())
            {
                roomClient.Send(msg);
            }
        }

        public void SendEnterBattle()
        {
            var msg = new Msg_CR_UserEnterBattle();
            msg.Flag = 1;
            SendToServer(msg);
        }

        public void SendInputDatas(int frameInx, List<int> datas)
        {
            var msg = new Msg_CR_InputDatas();
            Msg_CR_FrameInputData msgFrame = new Msg_CR_FrameInputData();
            msgFrame.FrameInx = frameInx;
            msgFrame.InputDatas.AddRange(datas);
            msg.InputFrames.Add(msgFrame);
            SendToServer(msg);
        }

        public void SendExitBattle()
        {
            var msg = new Msg_CR_UserExitBattle();
            msg.Flag = 1;
            SendToServer(msg);
        }

        public void SendBattleResult(EBattleResult eBattleResult)
        {
            var msg = new Msg_CR_BattleResult();
            msg.Result = eBattleResult;
            SendToServer(msg);
        }

        #endregion

        #region Receive

        internal void OnUserEnterBattle(Msg_RC_UserEnterBattle msg)
        {

        }

        internal void OnBattleStart(Msg_RC_BattleStart msg)
        {
            _startBattleMsg = true;
        }

        internal void OnInputDatas(Msg_RC_InputDatas msg)
        {
            _serverInputFrameQueue.Enqueue(msg.InputFrames[0]);
        }

        internal void OnUserExitBattle(Msg_RC_UserExitBattle msg)
        {
        }

        internal void OnBattleClose(Msg_RC_BattleClose msg)
        {
        }

        #endregion
    }
}
