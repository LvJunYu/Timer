using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using Spine.Unity;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame, EUIAutoSetupType.Create)]
    public class UICtrlSettlePlayersData : UICtrlInGameAnimationBase<UIViewSettlePlayersData>
    {
        private List<SettlePlayerData> _allPlayerDatas = new List<SettlePlayerData>();
        private List<UMCtlSettlePalyerDataItem> _allPlayDataItems = new List<UMCtlSettlePalyerDataItem>();
        private List<long> _likePlaysGuid = new List<long>();
        private Project _project;
        private Transform _moveLightParent;
        private Transform _palyergroupParent;
        private List<Vector3> _targetPosList = new List<Vector3>();
        private const string LightEffect = "M1EffectFinishGameFireWork";
        private UIParticleItem _ligtEffect;
        private bool _isCooperation;
        private List<UIParticleItem> _uiParticleItemlist = new List<UIParticleItem>();

        private List<RenderCamera> _camerasList = new List<RenderCamera>();

        private bool _firstOpen = true;
//        private int _coutnum = 0;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ExitBtn.onClick.AddListener(OnExitBtn);
            _cachedView.ReplayBtn.onClick.AddListener(OnRetryBtn);
            _moveLightParent = _cachedView.MoveLight.transform.parent;
            _palyergroupParent = _cachedView.PlayGroup[0].transform.parent;
            for (int i = 0; i < _cachedView.LightsImage.Length; i++)
            {
                UIParticleItem item = GameParticleManager.Instance.GetUIParticleItem(LightEffect,
                    _cachedView.LightsImage[i].rectTransform, _groupId);
                item.Particle.Play();
                _uiParticleItemlist.Add(item);
            }

            for (int i = 0; i < _cachedView.PlayerAvatarAnimation.Length; i++)
            {
                _cachedView.PlayerAvatarAnimation[i].skeletonDataAsset =
                    JoyResManager.Instance.GetAsset<SkeletonDataAsset>(EResType.SpineData, "SMainBoy0_SkeletonData",
                        (int) EResScenary.UIInGame);
                _cachedView.PlayerAvatarAnimation[i].Initialize(false);
                ChangePartsSpineView _avatarView = new ChangePartsSpineView();
                _avatarView.HomePlayerAvatarViewInit(_cachedView.PlayerAvatarAnimation[i]);
                RenderCamera _renderCamera =
                    RenderCameraManager.Instance.GetCamera(1.4f, _cachedView.PlayerAvatarAnimation[i].transform, 200,
                        360);
                _cachedView.PlayGroup[i].texture = _renderCamera.Texture;
//                _cachedView.PlayerAvatarAnimation[i].state.SetAnimation(0, "Idle1", true);
            }
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _allPlayerDatas = (List<SettlePlayerData>) parameter;
            _allPlayerDatas.Sort((a, b) =>
            {
                int anum = a.IsWin ? 1 : 0;
                int bnum = b.IsWin ? 1 : 0;
                return bnum - anum;
            });
            _likePlaysGuid.Clear();
            _cachedView.DataPanel.SetActiveEx(false);
            for (int i = 0; i < _uiParticleItemlist.Count; i++)
            {
                _uiParticleItemlist[i].Particle.Play();
            }

            _camerasList.Clear();
//            _coutnum = 10;
        }

        private void RefreshDataPanels()
        {
            _cachedView.AllLightImage.DOFade(1.0f, 1.0f).OnComplete(() =>
            {
                for (int i = 0; i < _uiParticleItemlist.Count; i++)
                {
                    _uiParticleItemlist[i].Particle.Stop();
                }

                _cachedView.DataPanel.SetActiveEx(true);
                bool mainPlayWin = false;
                for (int i = 0; i < _cachedView.CoorepationObj.Length; i++)
                {
                    _cachedView.CoorepationObj[i].SetActiveEx(_isCooperation);
                }

                for (int i = 0; i < _cachedView.BattleObj.Length; i++)
                {
                    _cachedView.BattleObj[i].SetActiveEx(!_isCooperation);
                }

                for (int i = 0; i < _allPlayerDatas.Count; i++)
                {
                    if (_allPlayerDatas[i].MainPlayID == _allPlayerDatas[i].PlayerId)
                    {
                        mainPlayWin = _allPlayerDatas[i].IsWin;
                    }

                    if (_allPlayerDatas[i].IsWin)
                    {
                        UMCtlSettlePalyerDataItem palyerDataItem =
                            UMPoolManager.Instance.Get<UMCtlSettlePalyerDataItem>(
                                _cachedView.WinContentTrans,
                                _cachedView.ResScenary);
                        palyerDataItem.SetItemData(_allPlayerDatas[i], _isCooperation);
                        palyerDataItem.Show();
                        palyerDataItem.MoveContent(i);
                        _allPlayDataItems.Add(palyerDataItem);
                    }
                    else
                    {
                        UMCtlSettlePalyerDataItem palyerDataItem =
                            UMPoolManager.Instance.Get<UMCtlSettlePalyerDataItem>(
                                _cachedView.LoseContentTrans,
                                _cachedView.ResScenary);
                        palyerDataItem.SetItemData(_allPlayerDatas[i], _isCooperation);
                        palyerDataItem.Show();
                        palyerDataItem.MoveContent(i);
                        _allPlayDataItems.Add(palyerDataItem);
                    }
                }

                _cachedView.WinTileImage.SetActiveEx(mainPlayWin);
                _cachedView.FailTileImage.SetActiveEx(!mainPlayWin);
                if (_isCooperation)
                {
                    if (mainPlayWin)
                    {
                        _cachedView.CoorepationFailObj.SetActiveEx(false);
                    }
                    else
                    {
                        _cachedView.CoorepationWinObj.SetActiveEx(false);
                    }
                }

                if (_project != null)
                {
                    _cachedView.TileText.text = _project.Name;
                }
            });
        }

        protected override void OnClose()
        {
            if (_likePlaysGuid.Count > 0)
            {
                RemoteCommands.WorldBattleEndUserLike(_likePlaysGuid, ret => { }, code => { });
            }

            for (int i = 0; i < _allPlayDataItems.Count; i++)
            {
                UMPoolManager.Instance.Free(_allPlayDataItems[i]);
            }

            _allPlayDataItems.Clear();
            for (int i = 0; i < _camerasList.Count; i++)
            {
                RenderCameraManager.Instance.FreeCamera(_camerasList[i]);
            }

            _firstOpen = false;
            base.OnClose();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
//            if (_isOpen)
//            {
//                _coutnum++;
////                if (_coutnum >= 10)
////                {
////                    for (int i = 0; i < _cachedView.PlayGroup.Length; i++)
////                    {
////                        if (i >= _allPlayerDatas.Count)
////                        {
////                        }
////                        else
////                        {
////                            PlayerBase player = TeamManager.Instance.Players[i];
////                            if (_allPlayerDatas[i].IsWin)
////                            {
////                                player.View.Animation
////                                    .PlayLoop("Victory", 1, 1);
////                            }
////                            else
////                            {
////                                player.View.Animation
////                                    .PlayLoop("Idle1", 1, 1);
////                            }
////
////                            if (i < _camerasList.Count)
////                            {
////                                _camerasList[i].SetOffsetPos(player.View.Trans.localPosition);
////                            }
////                        }
////                    }
////f
////                    _coutnum = 0;
////                }
//            }
            for (int i = 0; i < _cachedView.PlayerAvatarAnimation.Length; i++)
            {
                if (_cachedView.PlayerAvatarAnimation[i] != null)
                {
                    if (_firstOpen)
                    {
//                        _cachedView.PlayerAvatarAnimation[i].Update(ConstDefineGM2D.FixedDeltaTime * 0.5f);
                    }
                    else
                    {
                        _cachedView.PlayerAvatarAnimation[i].Update(ConstDefineGM2D.FixedDeltaTime);
                    }
                }
            }
        }

        private void OnExitBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlSettlePlayersData>();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
            GM2DGame.Instance.QuitGame(
                () => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                code => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                true
            );
        }

        private void OnRetryBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlSettlePlayersData>();
            if (_project != null)
            {
                RoomManager.Instance.SendRequestQuickPlay(EQuickPlayType.EQPT_Specific, _project.ProjectId);
            }
        }

        public void AddLikePlayer(long userguid)
        {
            _likePlaysGuid.Add(userguid);
        }

        public void RemoveLikePlayer(long userguid)
        {
            _likePlaysGuid.Remove(userguid);
        }

        public void SetProject(Project project)
        {
            _project = project;
            _isCooperation = false;
            if (_project.ProjectType == EProjectType.PT_Cooperation)
            {
                _isCooperation = true;
            }

            SetPlayerAniImage();
        }

        private void SetPlayerAniImage()
        {
            SetInitializedAni();
            int mvpindex = SetPalyerImage();
            _cachedView.PlayersLayoutGroup.SetLayoutHorizontal();
            _cachedView.PlayersLayoutGroup.CalculateLayoutInputHorizontal();
            SetMoveLightTweenPos(mvpindex);
            Sequence moveLight = DOTween.Sequence();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < _targetPosList.Count; j++)
                {
                    moveLight.Append(
                        _cachedView.MoveLight.rectTransform.DOLocalMoveX(_targetPosList[j].x, 0.1f).SetDelay(0.2f));
                }
            }

            Vector2 mvpPos = GetMVPPos(mvpindex);
            if (mvpPos != Vector2.zero)
            {
                moveLight.Append(
                    _cachedView.MoveLight.rectTransform
                        .DOLocalMoveX(mvpPos.x, 0.2f));
                for (int i = 0; i < 2; i++)
                {
                    moveLight.Append(
                        _cachedView.MoveLight.rectTransform
                            .DOLocalMoveX(mvpPos.x, 0.2f)
                            .SetDelay(1.5f));
                }
            }


            moveLight.OnComplete(() =>
            {
                if (_isCooperation)
                {
                    _cachedView.MoveLight.SetActiveEx(false);
                    _cachedView.AllLightImage.SetActiveEx(true);
                    RefreshDataPanels();
                }
                else
                {
                    _cachedView.LeftLight.SetActiveEx(true);
                    _cachedView.RightLight.SetActiveEx(true);
                    _cachedView.MoveLight.SetActiveEx(false);
                    _cachedView.AllLightImage.SetActiveEx(false);
                    _cachedView.MvpImage.SetActiveEx(true);
                    _cachedView.PlayersContentSizeFitter.SetEnableEx(false);
                    _cachedView.PlayersLayoutGroup.enabled = false;
                    if (mvpindex == -1)
                    {
                        mvpindex = 0;
                    }

                    _cachedView.PlayGroup[mvpindex].rectTransform.DOLocalMove(
                        _cachedView.PlayersLayoutGroup.transform.InverseTransformPoint(_cachedView.MvpImage
                            .transform
                            .position) + Vector3.up * 240.0f, 1.0f).OnComplete(RefreshDataPanels).PlayForward();
                }
            });
            moveLight.PlayForward();
        }

        private void SetInitializedAni()
        {
            _cachedView.AniPanel.SetActiveEx(true);
            _cachedView.PlayersContentSizeFitter.SetEnableEx(true);
            _cachedView.PlayersLayoutGroup.SetEnableEx(true);
            _cachedView.MoveLight.SetActiveEx(true);
            _cachedView.LeftLight.SetActiveEx(false);
            _cachedView.RightLight.SetActiveEx(false);
            _cachedView.AllLightImage.SetActiveEx(false);
            _cachedView.MvpImage.SetActiveEx(false);
        }

        private int SetPalyerImage()
        {
            int mvpindex = -1;
            for (int i = 0; i < _cachedView.PlayGroup.Length; i++)
            {
                if (i >= _allPlayerDatas.Count)
                {
                    _cachedView.PlayGroup[i].SetActiveEx(false);
                }
                else
                {
                    PlayerBase player = TeamManager.Instance.Players[i];
                    player.View.Trans.localPosition = new Vector3(1.0f, 1.0f, 0) * 20.0f * i;
                    _cachedView.PlayGroup[i].SetActiveEx(true);
                    if (_allPlayerDatas[i].IsWin)
                    {
//                        player.View.Animation
//                            .PlayLoop("Victory", 1, 1);
                        _cachedView.PlayerAvatarAnimation[i].state.SetAnimation(0, "Victory", true);
                    }
                    else
                    {
                        _cachedView.PlayerAvatarAnimation[i].state.SetAnimation(0, "Idle1", true);
//                        player.View.Animation
//                            .PlayLoop("Idle1", 1, 1);
                    }

//                    RenderCamera camera =
//                        RenderCameraManager.Instance.GetCamera(2.4f, player.View.Trans, 300,
//                            540);
//                    camera.SetOffsetPos(player.View.Trans.localPosition);
//                    _camerasList.Add(camera);
//                    _cachedView.PlayGroup[i].texture = camera.Texture;
                    _cachedView.PlayNameGroup[i].text = _allPlayerDatas[i].Name;
                    if (_allPlayerDatas[i].IsMvp)
                    {
                        mvpindex = i;
                    }
                }
            }

            return mvpindex;
        }

        private void SetMoveLightTweenPos(int mvpindex)
        {
            Canvas.ForceUpdateCanvases();
            _targetPosList.Clear();
            for (int i = 0; i < _cachedView.PlayGroup.Length; i++)
            {
                if (_cachedView.PlayGroup[i].transform.gameObject.activeSelf)
                {
                    Vector2 targetPos = _cachedView.MoveLight.rectTransform.localPosition;
                    targetPos.x =
                        _moveLightParent
                            .InverseTransformPoint(
                                _palyergroupParent.transform.TransformPoint(_cachedView.PlayGroup[i].transform
                                    .localPosition)).x;
                    _targetPosList.Add(targetPos);
                }
            }
        }

        private Vector2 GetMVPPos(int mvpindex)
        {
            Vector2 targetPos = Vector2.zero;
            if (!_isCooperation && mvpindex >= 0 && mvpindex < _cachedView.PlayGroup.Length)
            {
                targetPos = _cachedView.MoveLight.rectTransform.localPosition;
                targetPos.x =
                    _moveLightParent
                        .InverseTransformPoint(
                            _palyergroupParent.transform.TransformPoint(_cachedView.PlayGroup[mvpindex].transform
                                .localPosition)).x;
            }

            return targetPos;
        }
    }
}