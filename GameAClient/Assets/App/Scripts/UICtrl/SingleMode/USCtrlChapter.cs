/********************************************************************
** Filename : UICtrlSingleMode
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USCtrlChapter : USCtrlBase<USViewChapter>
    {
        private UMCtrlLevel[] _normalLevels;
        private UMCtrlLevel[] _bonusLevels;
        private UIParticleItem _travelEffect;
        private EResScenary _resScenary;

        private readonly string[] _islandSpriteName = new string[4]
            {"img_island", "img_island_ice", "img_island_three", "img_island_four"};

        private Sprite _islandSprite;
        private bool _isDoingAnimation;
        private Table_StandaloneChapter _tableStandaloneChapter;

        public RectTransform UITran
        {
            get { return _cachedView.Trans; }
        }

        public void RefreshInfo(Table_StandaloneChapter table, int chapterIndex, bool doPassAnimate = false)
        {
            JoyResManager.Instance.TryGetSprite(_islandSpriteName[chapterIndex - 1], out _islandSprite);
            if (_isDoingAnimation)
            {
                return;
            }
            _tableStandaloneChapter = table;
            if (_normalLevels == null)
            {
                _normalLevels = new UMCtrlLevel[9];
                for (int i = 0; i < 9; i++)
                {
                    _normalLevels[i] = new UMCtrlLevel();
                    _normalLevels[i].Init(_cachedView.NormalLevelPos[i], _resScenary);
                    _normalLevels[i].SetIslandImage(_islandSprite);
//					bool isDown = false;
//					if (i%2 == 0){isDown = true;}
//					_normalLevels[i].SetTween(isDown, Vector2.up*i*2);
                    var tableLevel = TableManager.Instance.GetStandaloneLevel(_tableStandaloneChapter.NormalLevels[i]);
                    if (tableLevel == null)
                    {
                        LogHelper.Error("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id, i);
                    }
                    else
                    {
                        _normalLevels[i].SetData(_tableStandaloneChapter.Id, i + 1, tableLevel);
                    }
                }
            }
            if (_bonusLevels == null)
            {
                _bonusLevels = new UMCtrlLevel[3];
                for (int i = 0; i < 3; i++)
                {
                    _bonusLevels[i] = new UMCtrlBonusLevel();
                    _bonusLevels[i].Init(_cachedView.BonusLevelPos[i], _resScenary);
//					bool isDown = false;
//					if (i%2 == 0){isDown = true;}
//					_bonusLevels[i].SetTween(isDown, Vector2.up*i*2);
                    var tableLevel = TableManager.Instance.GetStandaloneLevel(table.BonusLevels[i]);
                    if (tableLevel == null)
                    {
                        LogHelper.Error("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id, i);
                    }
                    else
                    {
                        _bonusLevels[i].SetData(table.Id, i + 1, tableLevel);
                    }
                }
            }

//			if (table.Id > AppData.Instance.AdventureData.UserData.AdventureUserProgress.SectionUnlockProgress) {
//				LockImage.SetActive (true);
//			} else 
            {
//				LockImage.SetActive(false);
                if (doPassAnimate)
                {
                    _isDoingAnimation = true;
                    CoroutineProxy.Instance.StartCoroutine(AnimationFlow());
                }
                else
                {
                    for (int i = 0, n = _normalLevels.Length; i < n; i++)
                    {
                        _normalLevels[i].RefreshInfo();
                    }
                    for (int i = 0, n = _bonusLevels.Length; i < n; i++)
                    {
                        var tableLevel = TableManager.Instance.GetStandaloneLevel(table.BonusLevels[i]);
                        if (tableLevel == null)
                        {
                            LogHelper.Error("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id,
                                i);
                        }
                        _bonusLevels[i].RefreshInfo();
                    }
                }
            }
        }

        private IEnumerator AnimationFlow()
        {
            var advData = AppData.Instance.AdventureData;
            if (advData.LastPlayedLevelType == EAdventureProjectType.APT_Bonus)
            {
                _bonusLevels[advData.LastPlayedLevelIdx - 1].RefreshInfo(true);
            }
            else
            {
                _normalLevels[advData.LastPlayedLevelIdx - 1].RefreshInfo(true);
                for (int i = 0; i < _bonusLevels.Length; i++)
                {
                    var bl = _bonusLevels[i];
                    bl.RefreshInfo(true);
                }
                int nextLevel;
                if (advData.TryGetNextNormalLevel(advData.LastPlayedChapterIdx, advData.LastPlayedLevelIdx,
                    out nextLevel))
                {
                    yield return new WaitForSeconds(1f);
                    _normalLevels[nextLevel - 1].RefreshInfo(true);
//				    yield return new WaitForSeconds(0.5f);
//				    var uiMain = SocialGUIManager.Instance.GetUI<UICtrlSingleMode>();
//				    if (uiMain.IsOpen
//				        && uiMain.CurrentChapter == _tableStandaloneChapter.Id
//				        && !SocialGUIManager.Instance.GetUI<UICtrlAdvLvlDetail>().IsOpen)
//				    {
//					    IntVec3 intVec3 = new IntVec3();
//                        intVec3.x = _tableStandaloneChapter.Id;
//                        intVec3.y = nextLevel;
//                        intVec3.z = 0;
//						SocialGUIManager.Instance.OpenUI<UICtrlAdvLvlDetail>(intVec3);
//				    }
                }
            }
            _isDoingAnimation = false;
            yield return null;
        }

        public void SetBonusLevelLockState(int level, bool isLock, bool playAnimation)
        {
            _cachedView.BonusLevelBlockGroup[level - 1].SetActiveEx(!isLock);
            if (!isLock && playAnimation)
            {
                SelectRoadFall(level - 1);
            }
        }

        private void SelectRoadFall(int AdventureIndex)
        {
            if (AdventureIndex == 0)
            {
                RoadFall(_cachedView.BonusLevelBlockImages1);
            }
            if (AdventureIndex == 1)
            {
                RoadFall(_cachedView.BonusLevelBlockImages2);
            }
            if (AdventureIndex == 2)
            {
                RoadFall(_cachedView.BonusLevelBlockImages3);
            }
        }

        private void RoadFall(Image[] roadImages)
        {
            for (int i = 0; i < roadImages.Length; i++)
            {
                float posY = roadImages[i].rectTransform.anchoredPosition.y;
                roadImages[i].rectTransform.anchoredPosition += Vector2.up * 200.0f;
                Tweener tweener = roadImages[i].rectTransform.DOAnchorPosY(posY, 1f);
                tweener.SetDelay(i);
            }
        }

        public void RefreshFriendProgress(int level, List<UserInfoDetail> friendsDataList)
        {
            if (_normalLevels != null && _normalLevels[level - 1] != null)
            {
                _normalLevels[level - 1].RefreshFriendProgress(friendsDataList);
            }
        }

        public void ClearFriendProgress()
        {
            if (_normalLevels == null) return;
            for (int i = 0; i < _normalLevels.Length; i++)
            {
                _normalLevels[i].ClearFriendProgress();
            }
        }

        public void SetResScenary(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }
    }
}