﻿/********************************************************************
** Filename : UICtrlSingleMode
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
	public class UICtrlSingleMode : UISocialCtrlBase<UIViewSingleMode>
    {
        #region 常量与字段
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
		private bool _dragging = false;
		/// <summary>
		/// 屏蔽玩家输入计时器
		/// </summary>
		private float _blockInputTimer;
        #endregion

        #region 属性
		/// <summary>
		/// 当前显示章节
		/// </summary>
		/// <value>The current chapter.</value>
		private int CurrentChapter {
			get {
				return _currentChapter;
			}
			set {
				if (_currentChapter != value) {
					_currentChapter = value;
					RefreshChapterInfo ();
				}
			}
		}

        #endregion

        #region 方法
        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
			_dragging = true;

			if (_currentChapter < 1) {
				CurrentChapter = 1;
			}
			if (_currentChapter > ChapterCnt) {
				CurrentChapter = ChapterCnt;
			}

			if (_currentChapter <= AppData.Instance.AdventureData.UserData.SectionList.Count) {
				if (AppData.Instance.AdventureData.UserData.SectionList [_currentChapter - 1].IsDirty) {
					AppData.Instance.AdventureData.UserData.SectionList [_currentChapter - 1].Request (
						LocalUser.Instance.UserGuid,
						_currentChapter,
						() => {
							RefreshChapterInfo ();
						}, null
					);
				}
			}
            SocialGUIManager.ShowGoldEnergyBar (true);
//			AppData.Instance.AdventureData.UserData.UserEnergyData.Request (
//				LocalUser.Instance.UserGuid,
//				() => {
//					RefreshEnergyInfo ();
//				}, null
//			);
//			RefreshEnergyInfo ();
			RefreshChapterInfo ();
        }

        protected override void OnClose()
        {
            base.OnClose();
			_dragging = false;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
			RegisterEvent(EMessengerType.OnChangeToAppMode, OnReturnToApp);
//            RegisterEvent(EMessengerType.OnAccountLoginStateChanged, OnEvent);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

			_cachedView.ReturnBtn.onClick.AddListener (OnReturnBtnClick);
			_cachedView.MatchBtn.onClick.AddListener (OnMatchBtnClick);
			_cachedView.TreasureBtn.onClick.AddListener (OnTreasureBtnClick);
			_cachedView.EncBtn.onClick.AddListener (OnEncBtnClick);
            _cachedView.NextBtn.onClick.AddListener (OnNextBtn);
            _cachedView.PrevBtn.onClick.AddListener (OnPrevBtn);

			int screenWidth = (int)_cachedView.ChapterScrollRect.GetComponent<RectTransform> ().GetWidth();
			float contentWidth = (screenWidth + ChapterDistance) / 2 + ChapterDistance * (ChapterCnt - 1);
			_cachedView.ChapterScrollRect.content.SetWidth (contentWidth);
			float basePos = (ChapterDistance - screenWidth) * 0.5f / (contentWidth - screenWidth);
			_maxDragPos = 1 + basePos;//ChapterDistance * ChapterCnt / contentWidth;
			for (int i = 0; i < ChapterCnt; i++) {
				_chapterRightNormalizedHorizontalPos[i] = 1 - (1 - basePos) / (ChapterCnt - 1) * (ChapterCnt - i - 1);
			}
			_cachedView.ChapterScrollRect.horizontalNormalizedPosition = basePos;
			_cachedView.ChapterScrollRect.OnBeginDragEvent.AddListener (OnBeginDrag);
			_cachedView.ChapterScrollRect.OnEndDragEvent.AddListener (OnEndDrag);
			_cachedView.ChapterScrollRect.onValueChanged.AddListener (OnValueChanged);
			_cachedView.LevelClickedCB = OnLevelClicked;

			_dragging = false;
			_blockInputTimer = 0f;
			_cachedView.InputBlock.enabled = false;

			_currentChapter = AppData.Instance.AdventureData.UserData.AdventureUserProgress.SectionUnlockProgress;
        }
			
		public override void OnUpdate ()
		{
			base.OnUpdate ();
			if (_currentChapter < 1) {
				CurrentChapter = 1;
			}
			if (_currentChapter > ChapterCnt) {
				CurrentChapter = ChapterCnt;
			}
			// chapter rollrect action
			float lerpSpeed = 0.2f;
			if (_blockInputTimer > 0) {
				_blockInputTimer -= Time.deltaTime;
				if (_blockInputTimer <= 0) {
					_cachedView.InputBlock.enabled = false;
				}
				lerpSpeed = 0.1f;
			}
			if (!_dragging) {
                if (Mathf.Abs (_cachedView.ChapterScrollRect.horizontalNormalizedPosition - _chapterRightNormalizedHorizontalPos [_currentChapter - 1]) > 0.001f) {
                    _cachedView.ChapterScrollRect.horizontalNormalizedPosition = 
					Mathf.Lerp (_cachedView.ChapterScrollRect.horizontalNormalizedPosition, 
                        _chapterRightNormalizedHorizontalPos [_currentChapter - 1],
                        lerpSpeed);
                }
			}

			if ((Input.touchCount == 0 && !Input.anyKey) && _cachedView.ChapterScrollRect.isForceReleased) {
				_cachedView.ChapterScrollRect.EnableDrag ();
			}
			///----------------------------------------------
		}

		/// <summary>
		/// 开始滚动卷轴，屏蔽玩家输入，直到_blockInputTimer计时器归零
		/// </summary>
		private void BeginChangeChapter () {
			if (_currentChapter <= AppData.Instance.AdventureData.UserData.SectionList.Count) {
				if (AppData.Instance.AdventureData.UserData.SectionList [_currentChapter - 1].IsDirty) {
					AppData.Instance.AdventureData.UserData.SectionList [_currentChapter - 1].Request (
						LocalUser.Instance.UserGuid,
						_currentChapter,
						() => {
							RefreshChapterInfo ();
						}, null
					);
				}
			}
			_blockInputTimer = 1f;
			_cachedView.ChapterScrollRect.ForceReleaseAndDisableDrag();
			_cachedView.InputBlock.enabled = true;
		}

		

		private void RefreshChapterInfo () {
			if (_currentChapter < 1) {
				_currentChapter = 1;
			}
			if (_currentChapter > ChapterCnt) {
				_currentChapter = ChapterCnt;
			}
			var tableChapter = TableManager.Instance.GetStandaloneChapter (_currentChapter);
			if (tableChapter == null)
				return;
			_cachedView.ChapterTitle.text = tableChapter.Name;
			if (_currentChapter > AppData.Instance.AdventureData.UserData.SectionList.Count) {
//				LogHelper.Error ("Can't get chapter info from server, chapter index is {0}.", _currentChapter);
//				return;
				_cachedView.StarNumber.text = "0 / 27";
			} else {
				_cachedView.StarNumber.text = string.Format ("{0} / {1}",
					AppData.Instance.AdventureData.UserData.SectionList [_currentChapter - 1].GotStarCnt,
					27
				);
			}

			if (_cachedView.Chapters [_currentChapter - 1] != null) {
				_cachedView.Chapters [_currentChapter - 1].RefreshInfo (tableChapter);
			}

            if (_currentChapter == 1) {
                _cachedView.PrevBtn.gameObject.SetActive (false);
            } else {
                _cachedView.PrevBtn.gameObject.SetActive (true);
            }
            if (_currentChapter == ChapterCnt) {
                _cachedView.NextBtn.gameObject.SetActive (false);
            } else {
                _cachedView.NextBtn.gameObject.SetActive (true);
            }
		}
        
        #region 接口
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }
			

		private void OnReturnBtnClick () {
			SocialGUIManager.Instance.ReturnToHome ();
		}

		private void OnMatchBtnClick () {
			SocialGUIManager.Instance.OpenUI<UICtrlModifyMatchMain>();
		}

		private void OnTreasureBtnClick () {
		}

		private void OnEncBtnClick () {
            SocialGUIManager.ShowReward (null);
		}

        private void OnNextBtn () {
            if (_currentChapter >= ChapterCnt) {
                _currentChapter = ChapterCnt;
                return;
            }
            _currentChapter += 1;
            RefreshChapterInfo ();
            BeginChangeChapter ();
            _dragging = false;
        }
        private void OnPrevBtn () {
            if (_currentChapter <= 1) {
                _currentChapter = 1;
                return;
            }
            _currentChapter -= 1;
            RefreshChapterInfo ();
            BeginChangeChapter ();
            _dragging = false;
        }

		/// <summary>
		/// 关卡被点击，从关卡图标按钮发出的消息调用
		/// </summary>
		/// <param name="param">Parameter.</param>
		private void OnLevelClicked (object param) {
            SocialGUIManager.Instance.OpenUI <UICtrlAdvLvlDetail> (param);
		}

		private void OnBeginDrag (PointerEventData eventData) {
			_dragging = true;
		}
		private void OnEndDrag (PointerEventData eventData) {
			_dragging = false;
		}
		private void OnValueChanged (Vector2 normalizedPos) {
			float x = _cachedView.ChapterScrollRect.horizontalNormalizedPosition;
			if (!_dragging)
				return;
			if (_blockInputTimer > 0)
				return;
			if (_currentChapter < 1) {
				CurrentChapter = 1;
			}
			if (_currentChapter > ChapterCnt) {
				CurrentChapter = ChapterCnt;
			}
			if (_currentChapter == 1 && x < _chapterRightNormalizedHorizontalPos [_currentChapter - 1]) {
				if (x < _minDragPos) {
					_cachedView.ChapterScrollRect.horizontalNormalizedPosition = _minDragPos;
				}
				return;
			}
			if (_currentChapter == ChapterCnt && x > _chapterRightNormalizedHorizontalPos [_currentChapter - 1]) {
				if (x > _maxDragPos) {
					_cachedView.ChapterScrollRect.horizontalNormalizedPosition = _maxDragPos;
				}
				return;
			}
			if (Mathf.Abs (x - _chapterRightNormalizedHorizontalPos [_currentChapter - 1]) > (200 / _cachedView.ChapterScrollRect.content.GetWidth ())) {
				_currentChapter += x > _chapterRightNormalizedHorizontalPos [_currentChapter - 1] ? 1 : -1;
				RefreshChapterInfo ();
				BeginChangeChapter ();
				_dragging = false;
			}
		}

		private void OnPointUp (PointerEventData eventData) {
			if (_cachedView.ChapterScrollRect.isForceReleased) {
				_cachedView.ChapterScrollRect.EnableDrag ();
			}
		}

		private void OnReturnToApp () {
            if (!_isOpen)
                return;
//            _cachedView.ChapterScrollRect.horizontalNormalizedPosition = _chapterRightNormalizedHorizontalPos[_currentChapter - 1];
			RefreshChapterInfo ();
			if (_currentChapter <= AppData.Instance.AdventureData.UserData.SectionList.Count) {
				if (AppData.Instance.AdventureData.UserData.SectionList [_currentChapter - 1].IsDirty) {
					AppData.Instance.AdventureData.UserData.SectionList [_currentChapter - 1].Request (
						LocalUser.Instance.UserGuid,
						_currentChapter,
						() => {
							RefreshChapterInfo ();
						}, null
					);
				}
			}
		}
        #endregion 接口
        #endregion

    }
}
