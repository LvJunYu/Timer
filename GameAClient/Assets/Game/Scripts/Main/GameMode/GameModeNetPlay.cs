/********************************************************************
** Filename : GameModeNetPlay
** Author : Dong
** Date : 2017/6/23 星期五 下午 4:29:56
** Summary : GameModeNetPlay
***********************************************************************/

using System;
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

        public void SendInputDatas(List<int> datas)
        {
            var msg = new Msg_CR_InputDatas();
            msg.InputDatas.AddRange(datas);
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

        internal void OnBattleOpen(Room room)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Format("请求进入关卡"));
            var project = new Project();
            project.Request(room.ProjectGuid,
                () => project.RequestPlay(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.RequestPlayMultiCooperation(project);
                    SocialGUIManager.Instance.ChangeToGameMode();
                    //成功进入战场
                    SendEnterBattle();
                },
                    error =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        SocialGUIManager.ShowPopupDialog("进入关卡失败");
                    }),
                error =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("进入关卡失败");
                });
        }

        internal void OnUserEnterBattle(Msg_RC_UserEnterBattle msg)
        {

        }

        internal void OnBattleStart(Msg_RC_BattleStart msg)
        {
            _startBattleMsg = true;
        }

        internal void OnInputDatas(Msg_RC_InputDatas msg)
        {
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
