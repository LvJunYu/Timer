using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public abstract class GameModePlayRecord : GameModeBase
    {
        protected int _inputDataReadInx;

        public virtual Record Record
        {
            get { return _record; }
        }

        public int CurrentFrameCount
        {
            get { return GameRun.Instance.LogicFrameCnt; }
        }

        public float SpeedMultiple
        {
            get { return GM2DGame.Instance.GamePlaySpeed; }
            set { GM2DGame.Instance.SetGamePlaySpeed(value); }
        }

        public override bool Init(Project project, object param, GameManager.EStartType startType,
            MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            _gameRunMode = EGameRunMode.PlayRecord;
            return true;
        }

        public override IEnumerator InitByStep()
        {
            GameRun.Instance.ChangeState(ESceneState.Play);
            InitUI();
            InitGame();
            yield return null;
        }


        public override void Update()
        {
            if (!_run)
            {
                return;
            }
            GameRun.Instance.Update();
            while (GameRun.Instance.LogicTimeSinceGameStarted < GameRun.Instance.GameTimeSinceGameStarted)
            {
                if (GameRun.Instance.IsPlaying && null != PlayerManager.Instance.MainPlayer)
                {
                    LocalPlayerInput localPlayerInput = PlayerManager.Instance.MainPlayer.Input as LocalPlayerInput;
                    if (localPlayerInput != null)
                    {
                        localPlayerInput.PrepareForApplyInput();
                        while (_inputDataReadInx < _inputDatas.Count - 1
                               && _inputDatas[_inputDataReadInx] == GameRun.Instance.LogicFrameCnt)
                        {
                            _inputDataReadInx++;
                            localPlayerInput.ApplyKeyChangeCode(_inputDatas[_inputDataReadInx]);
                            _inputDataReadInx++;
                        }
                    }
                }
                GameRun.Instance.UpdateLogic(ConstDefineGM2D.FixedDeltaTime);
            }
        }

        public override void OnGameFailed()
        {
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(2f, () =>
            {
                if (GameManager.Instance.CurrentGame != null)
                {
                    SocialApp.Instance.ReturnToApp();
                }
            }));
        }

        public override void OnGameSuccess()
        {
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(2f, () =>
            {
                if (GameManager.Instance.CurrentGame != null)
                {
                    SocialApp.Instance.ReturnToApp();
                }
            }));
        }

        protected override bool InitRecord()
        {
            if (!base.InitRecord()) return false;
            _inputDatas.AddRange(_gm2drecordData.Data);
            return true;
        }

        protected virtual void InitUI()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSceneState>();
            SocialGUIManager.Instance.OpenUI<UICtrlGameScreenEffect>();
            SocialGUIManager.Instance.OpenUI<UICtrlRecordPlayControl>();
            InputManager.Instance.HideGameInput();
        }

        protected virtual void InitGame()
        {
            MainPlayer mainPlayer = PlayMode.Instance.MainPlayer;
            if (mainPlayer == null)
                return;
            // todo set data

            ChangePartsSpineView view = mainPlayer.View as ChangePartsSpineView;
            if (view == null)
                return;
            int headId = 1;
            int upperId = 1;
            int lowerId = 1;
            int appendageId = 1;

            if (_gm2drecordData != null && _gm2drecordData.Avatar != null)
            {
                headId = _gm2drecordData.Avatar.Head;
                upperId = _gm2drecordData.Avatar.Upper;
                lowerId = _gm2drecordData.Avatar.Lower;
                appendageId = _gm2drecordData.Avatar.Appendage;
            }
            //          view.SetParts (2, SpinePartsDefine.ESpineParts.Head);
            if (LocalUser.Instance.UsingAvatarData.Head != null)
            {
                view.SetParts(headId, SpinePartsHelper.ESpineParts.Head);
            }
            if (LocalUser.Instance.UsingAvatarData.Upper != null)
            {
                view.SetParts(upperId, SpinePartsHelper.ESpineParts.Upper);
            }
            if (LocalUser.Instance.UsingAvatarData.Lower != null)
            {
                view.SetParts(lowerId, SpinePartsHelper.ESpineParts.Lower);
            }
            if (LocalUser.Instance.UsingAvatarData.Appendage != null)
            {
                view.SetParts(appendageId, SpinePartsHelper.ESpineParts.Appendage);
            }
        }
    }
}