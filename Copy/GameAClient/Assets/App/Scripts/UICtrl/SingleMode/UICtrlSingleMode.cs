﻿/********************************************************************
** Filename : UICtrlSingleMode
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UISingleMode, EUIAutoSetupType.Create)]
    public class UICtrlSingleMode : UICtrlAnimationBase<UIViewSingleMode>
    {
        /// <summary>
        /// 每个章节界面间的水平距离
        /// </summary>
        public const int ChapterDistance = 1770;

        /// <summary>
        /// 总章节数
        /// </summary>
        public const int ChapterCnt = 4;

        /// <summary>
        /// 当前显示章节
        /// </summary>
        private int _currentChapter = 1;

        /// <summary>
        /// 各章节界面显示时 卷轴的正确标准位置（0-1）
        /// </summary>
        private float[] _chapterRightNormalizedHorizontalPos = new float[ChapterCnt];

        /// <summary>
        /// 卷轴能滚动的最小标准位置
        /// </summary>
        private float _minDragPos = 0;

        /// <summary>
        /// 卷轴能滚动的最大标准位置
        /// </summary>
        private float _maxDragPos;

        /// <summary>
        /// 是否正在拖拽
        /// </summary>
        private bool _dragging;

        /// <summary>
        /// 屏蔽玩家输入计时器
        /// </summary>
        private float _blockInputTimer;

        private USCtrlChapter[] _chapterAry;

        private UIParticleItem[] _uiParticleItemAry;
        private bool _pushGoldEnergyStyle;
        private List<AdvProgressData> _advProgressList;
        private Vector2 _preBtnPos;
        private Vector2 _nextBtnPos;

        /// <summary>
        /// 当前显示章节
        /// </summary>
        /// <value>The current chapter.</value>
        public int CurrentChapter
        {
            private set
            {
                if (_currentChapter != value)
                {
                    _currentChapter = value;
                    RefreshChapterInfo();
                }
            }
            get { return _currentChapter; }
        }

        public void SetChapterBonusLevelLockState(int chapter, int levelInx, bool isLock, bool playAnimation = false)
        {
            _chapterAry[chapter - 1].SetBonusLevelLockState(levelInx, isLock, playAnimation);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _dragging = true;
            if (_currentChapter <= 0)
            {
                _currentChapter = AppData.Instance.AdventureData.UserData.AdventureUserProgress.SectionUnlockProgress;
            }

            if (_currentChapter < 1)
            {
                CurrentChapter = 1;
            }
            if (_currentChapter > ChapterCnt)
            {
                CurrentChapter = ChapterCnt;
            }
            var curChapter = _currentChapter;
            _currentChapter = -1;
            CurrentChapter = curChapter;
            if (_currentChapter <= AppData.Instance.AdventureData.UserData.SectionList.Count)
            {
                if (AppData.Instance.AdventureData.UserData.SectionList[_currentChapter - 1].IsDirty)
                {
                    AppData.Instance.AdventureData.UserData.SectionList[_currentChapter - 1].Request(
                        LocalUser.Instance.UserGuid,
                        _currentChapter,
                        () =>
                        {
                            if (_cachedView)
                            {
                                RefreshChapterInfo();
                            }
                        }, null
                    );
                }
            }
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>()
                    .PushStyle(UICtrlGoldEnergy.EStyle.EnergyGoldDiamond);
                _pushGoldEnergyStyle = true;
            }
//			AppData.Instance.AdventureData.UserData.UserEnergyData.Request (
//				LocalUser.Instance.UserGuid,
//				() => {
//					RefreshEnergyInfo ();
//				}, null
//			);
//			RefreshEnergyInfo ();
            RefreshChapterInfo();
//            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.ModifyMatch))
//            {
//                _cachedView.MatchBtn.gameObject.SetActive(true); //todo
//            }
//            else
            {
                _cachedView.MatchBtn.gameObject.SetActive(false);
            }
            //打开匹配挑战
#if UNITY_EDITOR
//	        _cachedView.MatchBtn.SetActiveEx(true);
#endif
        }

        protected override void OnClose()
        {
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }
            _dragging = false;
            if (null != _uiParticleItemAry)
            {
                for (int i = 0; i < _uiParticleItemAry.Length; i++)
                {
                    var particle = _uiParticleItemAry[i];
                    if (particle != null)
                    {
                        particle.Particle.Stop();
                    }
                }
            }
            base.OnClose();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnReturnToApp);
//            RegisterEvent(EMessengerType.OnAccountLoginStateChanged, OnEvent);
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.TitleRtf, EAnimationType.MoveFromUp, new Vector3(0, 100, 0), 0.17f);
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.BGRtf, EAnimationType.Fade);
            SetPart(_cachedView.LeftBtnRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.RightBtnRtf, EAnimationType.MoveFromDown);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtnClick);
            _cachedView.MatchBtn.onClick.AddListener(OnMatchBtnClick);
            //_cachedView.TreasureBtn.onClick.AddListener (OnTreasureBtnClick);
            _cachedView.EncBtn.onClick.AddListener(OnEncBtnClick);
            _cachedView.NextBtn.onClick.AddListener(OnNextBtn);
            _cachedView.PrevBtn.onClick.AddListener(OnPrevBtn);

            int screenWidth = (int) _cachedView.ChapterScrollRect.GetComponent<RectTransform>().GetWidth();
            float contentWidth = (screenWidth + ChapterDistance) / 2 + ChapterDistance * (ChapterCnt - 1);
            _cachedView.ChapterScrollRect.content.SetWidth(contentWidth);
            float basePos = (ChapterDistance - screenWidth) * 0.5f / (contentWidth - screenWidth);
            _maxDragPos = 1 + basePos; //ChapterDistance * ChapterCnt / contentWidth;
            for (int i = 0; i < ChapterCnt; i++)
            {
                _chapterRightNormalizedHorizontalPos[i] = 1 - (1 - basePos) / (ChapterCnt - 1) * (ChapterCnt - i - 1);
            }
            _cachedView.ChapterScrollRect.horizontalNormalizedPosition = basePos;
            _cachedView.ChapterScrollRect.OnBeginDragEvent.AddListener(OnBeginDrag);
            _cachedView.ChapterScrollRect.OnEndDragEvent.AddListener(OnEndDrag);
            _cachedView.ChapterScrollRect.onValueChanged.AddListener(OnValueChanged);

            _dragging = false;
            _blockInputTimer = 0f;
            _cachedView.InputBlock.enabled = false;

            _cachedView.MatchBtn.SetActiveEx(false);

            _cachedView.SetActiveEx(false);
            _chapterAry = new USCtrlChapter[_cachedView.Chapters.Length];
            for (int i = 0; i < _chapterAry.Length; i++)
            {
                _chapterAry[i] = new USCtrlChapter();
                _chapterAry[i].SetResScenary(ResScenary);
                _chapterAry[i].Init(_cachedView.Chapters[i]);
            }
            _cachedView.ChapterBg[_currentChapter - 1].gameObject.SetActive(true);
            _cachedView.ChapterScrollRect.horizontalNormalizedPosition =
                _chapterRightNormalizedHorizontalPos[_currentChapter - 1];
            _uiParticleItemAry = new UIParticleItem[_cachedView.Chapters.Length];
            _preBtnPos = _cachedView.PreBtnRtf.anchoredPosition;
            _nextBtnPos = _cachedView.NextBtnRtf.anchoredPosition;
        }

        public override void OnUpdate()
        {
            _cachedView.NextBtnRtf.anchoredPosition = _nextBtnPos + new Vector2(3f, 0) * Mathf.Sin(Time.time * 3f);
            _cachedView.PreBtnRtf.anchoredPosition = _preBtnPos - new Vector2(3f, 0) * Mathf.Sin(Time.time * 3f);
            base.OnUpdate();
            if (_currentChapter < 1)
            {
                CurrentChapter = 1;
            }
            if (_currentChapter > ChapterCnt)
            {
                CurrentChapter = ChapterCnt;
            }
            // chapter rollrect action
            float lerpSpeed = 0.2f;
            if (_blockInputTimer > 0)
            {
                _blockInputTimer -= Time.deltaTime;
                if (_blockInputTimer <= 0)
                {
                    _cachedView.InputBlock.enabled = false;
                }
                lerpSpeed = 0.1f;
            }
            if (!_dragging)
            {
                if (Mathf.Abs(_cachedView.ChapterScrollRect.horizontalNormalizedPosition -
                              _chapterRightNormalizedHorizontalPos[_currentChapter - 1]) > 0.001f)
                {
                    _cachedView.ChapterScrollRect.horizontalNormalizedPosition =
                        Mathf.Lerp(_cachedView.ChapterScrollRect.horizontalNormalizedPosition,
                            _chapterRightNormalizedHorizontalPos[_currentChapter - 1],
                            lerpSpeed);
                }
            }

            if ((Input.touchCount == 0 && !Input.anyKey) && _cachedView.ChapterScrollRect.isForceReleased)
            {
                _cachedView.ChapterScrollRect.EnableDrag();
            }
        }

        /// <summary>
        /// 开始滚动卷轴，屏蔽玩家输入，直到_blockInputTimer计时器归零
        /// </summary>
        private void BeginChangeChapter()
        {
            if (_currentChapter <= AppData.Instance.AdventureData.UserData.SectionList.Count)
            {
                if (AppData.Instance.AdventureData.UserData.SectionList[_currentChapter - 1].IsDirty)
                {
                    AppData.Instance.AdventureData.UserData.SectionList[_currentChapter - 1].Request(
                        LocalUser.Instance.UserGuid,
                        _currentChapter,
                        () => { RefreshChapterInfo(); }, null
                    );
                }
            }
            _blockInputTimer = 1f;
            _cachedView.ChapterScrollRect.ForceReleaseAndDisableDrag();
            _cachedView.InputBlock.enabled = true;
        }

        private void RefreshChapterInfo(bool doPassAnimate = false)
        {
            if (_currentChapter < 1)
            {
                _currentChapter = 1;
            }
            if (_currentChapter > ChapterCnt)
            {
                _currentChapter = ChapterCnt;
            }
            var tableChapter = TableManager.Instance.GetStandaloneChapter(_currentChapter);
            if (tableChapter == null)
                return;
            _cachedView.ChapterTitle.text = tableChapter.Name;
            if (_currentChapter > AppData.Instance.AdventureData.UserData.SectionList.Count)
            {
//				LogHelper.Error ("Can't get chapter info from server, chapter index is {0}.", _currentChapter);
//				return;
                _cachedView.StarNumber.text = "0 / 27";
            }
            else
            {
                _cachedView.StarNumber.text = string.Format("{0} / {1}",
                    AppData.Instance.AdventureData.UserData.SectionList[_currentChapter - 1].GotStarCnt,
                    27
                );
            }

            if (_chapterAry[_currentChapter - 1] != null)
            {
                _chapterAry[_currentChapter - 1].RefreshInfo(tableChapter, _currentChapter, doPassAnimate);

                if (null != _uiParticleItemAry)
                {
                    for (int i = 0; i < _uiParticleItemAry.Length; i++)
                    {
                        var uiParticle = _uiParticleItemAry[i];
                        if (_currentChapter - 1 == i)
                        {
                            if (uiParticle != null)
                            {
                                uiParticle.Particle.Play();
                            }
                            else
                            {
                                uiParticle = GameParticleManager.Instance.GetUIParticleItem(
                                    ParticleNameConstDefineGM2D.SingleModeBgEffect + (i + 1),
                                    _chapterAry[_currentChapter - 1].UITran, _groupId);
                                uiParticle.Particle.Trans.localPosition = new Vector3(ChapterDistance / 2f, 0, 0);
                                _uiParticleItemAry[i] = uiParticle;
                                uiParticle.Particle.Play();
                            }
                        }
                        else
                        {
                            if (uiParticle != null)
                            {
                                uiParticle.Particle.Stop();
                            }
                        }
                    }
                }
            }

            if (_currentChapter == 1)
            {
                _cachedView.PrevBtn.gameObject.SetActive(false);
            }
            else
            {
                _cachedView.PrevBtn.gameObject.SetActive(true);
            }
            if (_currentChapter == ChapterCnt)
            {
                _cachedView.NextBtn.gameObject.SetActive(false);
            }
            else
            {
                _cachedView.NextBtn.gameObject.SetActive(true);
            }
            RefreshFriendProgress(_currentChapter, _currentChapter);
        }

        private void RefreshFriendProgress(int startChapter, int endChapter)
        {
            for (int i = 0; i < _chapterAry.Length; i++)
            {
                _chapterAry[i].ClearFriendProgress();
            }
            AppData.Instance.AdventureData.FriendsAdvProgressList.Request(startChapter, 1, endChapter, 9, () =>
            {
                _advProgressList = AppData.Instance.AdventureData.FriendsAdvProgressList.AdvProgressDataList;
                if (_advProgressList == null)
                {
                    LogHelper.Error("_advProgressList == null");
                    return;
                }
                for (int i = 0; i < _advProgressList.Count; i++)
                {
                    int chapter = _advProgressList[i].Section;
                    _chapterAry[chapter - 1].RefreshFriendProgress(_advProgressList[i].Level,
                        _advProgressList[i].FriendsDetailDataList);
                }
            }, code => { });
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        private void OnReturnBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlSingleMode>();
        }

        private void OnMatchBtnClick()
        {
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.ModifyMatch))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlModifyMatchMain>();
            }
        }

        private void OnTreasureBtnClick()
        {
        }

        private void OnEncBtnClick()
        {
            SocialGUIManager.ShowReward(null);
        }

        private void OnNextBtn()
        {
            if (_currentChapter >= ChapterCnt)
            {
                _currentChapter = ChapterCnt;
                return;
            }
            FadeOutChanterBg(_cachedView.ChapterBg[_currentChapter - 1]);
            _currentChapter += 1;
            FadeInChanterBg(_cachedView.ChapterBg[_currentChapter - 1]);
            RefreshChapterInfo();
            BeginChangeChapter();
            _dragging = false;
        }

        private void OnPrevBtn()
        {
            if (_currentChapter <= 1)
            {
                _currentChapter = 1;
                return;
            }
            FadeOutChanterBg(_cachedView.ChapterBg[_currentChapter - 1]);
            _currentChapter -= 1;
            FadeInChanterBg(_cachedView.ChapterBg[_currentChapter - 1]);
            RefreshChapterInfo();
            BeginChangeChapter();
            _dragging = false;
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;
        }

        private void OnValueChanged(Vector2 normalizedPos)
        {
            float x = _cachedView.ChapterScrollRect.horizontalNormalizedPosition;
            if (!_dragging)
                return;
            if (_blockInputTimer > 0)
                return;
            if (_currentChapter < 1)
            {
                CurrentChapter = 1;
            }
            if (_currentChapter > ChapterCnt)
            {
                CurrentChapter = ChapterCnt;
            }
            if (_currentChapter == 1 && x < _chapterRightNormalizedHorizontalPos[_currentChapter - 1])
            {
                if (x < _minDragPos)
                {
                    _cachedView.ChapterScrollRect.horizontalNormalizedPosition = _minDragPos;
                }
                return;
            }
            if (_currentChapter == ChapterCnt && x > _chapterRightNormalizedHorizontalPos[_currentChapter - 1])
            {
                if (x > _maxDragPos)
                {
                    _cachedView.ChapterScrollRect.horizontalNormalizedPosition = _maxDragPos;
                }
                return;
            }
            if (Mathf.Abs(x - _chapterRightNormalizedHorizontalPos[_currentChapter - 1]) >
                (200 / _cachedView.ChapterScrollRect.content.GetWidth()))
            {
                FadeOutChanterBg(_cachedView.ChapterBg[_currentChapter - 1]);
                _currentChapter += x > _chapterRightNormalizedHorizontalPos[_currentChapter - 1] ? 1 : -1;
                FadeInChanterBg(_cachedView.ChapterBg[_currentChapter - 1]);
                RefreshChapterInfo();
                BeginChangeChapter();
                _dragging = false;
            }
        }

        private void OnPointUp(PointerEventData eventData)
        {
            if (_cachedView.ChapterScrollRect.isForceReleased)
            {
                _cachedView.ChapterScrollRect.EnableDrag();
            }
        }

        private void OnReturnToApp()
        {
            if (!_isViewCreated || !_isOpen)
            {
                return;
            }
//            _cachedView.ChapterScrollRect.horizontalNormalizedPosition = _chapterRightNormalizedHorizontalPos[_currentChapter - 1];
            var advData = AppData.Instance.AdventureData;
            RefreshChapterInfo(advData.LastAdvSuccess);
            if (_currentChapter <= advData.UserData.SectionList.Count)
            {
                if (advData.UserData.SectionList[_currentChapter - 1].IsDirty)
                {
                    advData.UserData.SectionList[_currentChapter - 1].Request(
                        LocalUser.Instance.UserGuid,
                        _currentChapter,
                        () =>
                        {
                            if (_cachedView)
                            {
                                RefreshChapterInfo();
                            }
                        }, null
                    );
                }
            }

//            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.ModifyMatch))
//            {
//                _cachedView.MatchBtn.gameObject.SetActive(true);
//            }
//            else
//            {
//                _cachedView.MatchBtn.gameObject.SetActive(false);
//            }
        }

        private void FadeOutChanterBg(CanvasGroup chapterBg)
        {
            chapterBg.alpha = 1.0f;
            chapterBg.DOFade(0, 1);
            chapterBg.gameObject.SetActive(false);
        }

        private void FadeInChanterBg(CanvasGroup chapterBg)
        {
            chapterBg.gameObject.SetActive(true);
            chapterBg.alpha = 0.0f;
            chapterBg.DOFade(1, 1);
        }

        protected override void OnDestroy()
        {
            _uiParticleItemAry = null;
            base.OnDestroy();
        }
    }
}