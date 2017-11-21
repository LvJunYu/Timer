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
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlLevel : UMCtrlBase<UMViewlLevel>
    {
        /// <summary>
        /// 章节id
        /// </summary>
        private int _chapterId;

        /// <summary>
        /// 关卡在章节中的序号
        /// </summary>
        private int _levelIdx;

        private bool _isBonus;
        private Table_StandaloneLevel _tableLevel;
        private int _lastStarCount;
        private EState _state = EState.None;
        private Sequence _sequence;
        private UserInfoDetail[] _friends;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.LevelBtn.onClick.AddListener(OnClick);
            if (null != _cachedView.Current)
            {
                _cachedView.Current.onClick.AddListener(OnClick);
            }
            _friends = new UserInfoDetail[_cachedView.FriendHeadButtonBtns.Length];
            for (int i = 0; i < _cachedView.FriendHeadButtonBtns.Length; i++)
            {
                var inx = i;
                _cachedView.FriendHeadButtonBtns[i].onClick.AddListener(() =>
                {
                    if (_friends[inx] != null)
                    {
                        SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_friends[inx]);
                    }
                });
            }
        }

        public void SetData(int chapterId, int levelIdx, Table_StandaloneLevel tableLevel)
        {
            _chapterId = chapterId;
            _tableLevel = tableLevel;
//            Debug.Log ("level refresh info chapter: " + _chapterId + " level: " + levelIdx);
            _isBonus = tableLevel.Type != 0;
            _levelIdx = levelIdx;
        }

        public void RefreshInfo(bool playAnimation = false)
        {
            _cachedView.LevelTitle.text = _tableLevel.Name;
            CoroutineProxy.Instance.StartCoroutine(RefreshView(playAnimation));
        }

        private IEnumerator RefreshView(bool playAnimation)
        {
            var advData = AppData.Instance.AdventureData;
            var state = GetState();
            var groupId = SocialGUIManager.Instance.GetUI<UICtrlSingleMode>().GroupId;
            // normal level
            if (!_isBonus)
            {
                if (state == EState.Lock)
                {
                    StopTween();
                    _cachedView.Current.gameObject.SetActive(false);
                    _cachedView.Active.SetActive(false);
                    _cachedView.Disactive.SetActive(true);
                    _lastStarCount = 0;
                    for (int i = 0; i < _cachedView.StarLightAry.Length; i++)
                    {
                        _cachedView.StarLightAry[i].SetActiveEx(false);
                    }
                    for (int i = 0; i < _cachedView.StarDarkAry.Length; i++)
                    {
                        _cachedView.StarDarkAry[i].SetActiveEx(false);
                    }
                }
                else if (state == EState.Unlock)
                {
                    if (playAnimation && _state != state)
                    {
                        var item = GameParticleManager.Instance.GetUIParticleItem(
                            ParticleNameConstDefineGM2D.SingleModeNormalLevelUnlock,
                            _cachedView.Disactive.transform, groupId);
                        item.Particle.Play(4f);
                        yield return new WaitForSeconds(0.2f);
                    }
                    _cachedView.Current.gameObject.SetActive(true);
                    _cachedView.Active.SetActive(false);
                    _cachedView.Disactive.SetActive(false);
                    for (int i = 0; i < _cachedView.StarLightAry.Length; i++)
                    {
                        _cachedView.StarLightAry[i].SetActiveEx(false);
                    }
                    for (int i = 0; i < _cachedView.StarDarkAry.Length; i++)
                    {
                        _cachedView.StarDarkAry[i].SetActiveEx(true);
                    }
                    _lastStarCount = 0;
                    SetTween();
                }
                else
                {
                    StopTween();
                    var levelData =
                        advData.GetAdventureUserLevelDataDetail(_chapterId, EAdventureProjectType.APT_Normal,
                            _levelIdx);
                    int starCnt = levelData.SimpleData.GotStarCnt;

                    if (playAnimation)
                    {
                        for (int i = _lastStarCount; i < starCnt; i++)
                        {
                            var item = GameParticleManager.Instance.GetUIParticleItem(
                                ParticleNameConstDefineGM2D.SingleModeGetStar,
                                _cachedView.StarDarkAry[i], groupId);
                            item.Particle.Play(4f);
                            var inx = i;
                            CoroutineProxy.Instance.StartCoroutine(
                                CoroutineProxy.RunWaitForSeconds(0.2f, () =>
                                    {
                                        _cachedView.StarLightAry[inx].SetActiveEx(true);
                                        _cachedView.StarDarkAry[inx].SetActiveEx(false);
                                    }
                                ));
                            yield return new WaitForSeconds(0.2f);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _cachedView.StarLightAry.Length; i++)
                        {
                            _cachedView.StarLightAry[i].SetActiveEx(i < starCnt);
                            _cachedView.StarDarkAry[i].SetActiveEx(i >= starCnt);
                        }
                    }
                    if (playAnimation && _state != state)
                    {
                        var item = GameParticleManager.Instance.GetUIParticleItem(
                            ParticleNameConstDefineGM2D.SingleModeNormalLevelComplete,
                            _cachedView.Active.transform, groupId);
                        item.Particle.Play(4f);
                        yield return new WaitForSeconds(0.2f);
                    }
                    _cachedView.Current.gameObject.SetActive(false);
                    _cachedView.Active.SetActive(true);
                    _cachedView.Disactive.SetActive(false);
                    _lastStarCount = starCnt;
                }
            }
            else
            {
                if (state == EState.Lock)
                {
                    _cachedView.Active.SetActive(false);
                    _cachedView.Disactive.SetActive(true);
                    _cachedView.StarLightAry[0].SetActiveEx(false);
                    _cachedView.LightImage.gameObject.SetActive(false);
                    SocialGUIManager.Instance.GetUI<UICtrlSingleMode>()
                        .SetChapterBonusLevelLockState(_chapterId, _levelIdx, true);
                }
                else if (state == EState.Unlock)
                {
                    _cachedView.Active.SetActive(true);
                    _cachedView.Disactive.SetActive(false);
                    _cachedView.StarLightAry[0].SetActiveEx(false);
                    _cachedView.LightImage.gameObject.SetActive(true);
                    var needAnimation = playAnimation && _state != state;
                    SocialGUIManager.Instance.GetUI<UICtrlSingleMode>()
                        .SetChapterBonusLevelLockState(_chapterId, _levelIdx, false, needAnimation);
                }
                else
                {
                    _cachedView.Active.SetActive(true);
                    _cachedView.Disactive.SetActive(false);
                    _cachedView.StarLightAry[0].SetActiveEx(true);
                    _cachedView.LightImage.gameObject.SetActive(false);
                }
                if (AppData.Instance.AdventureData.UserData.SectionList.Count > (_chapterId - 1))
                {
                    _cachedView.StartText.text = string.Format("{0}",
                        Mathf.Clamp(AppData.Instance.AdventureData.UserData.SectionList[_chapterId - 1].GotStarCnt, 0,
                            9 * _tableLevel.Type)
                    );
                    _cachedView.StartText2.text = string.Format("/{0}",
                        9 * _tableLevel.Type
                    );
                }
                else
                {
                    _cachedView.StartText.text = "0";
                    _cachedView.StartText2.text = string.Format("/{0}",
                        9 * _tableLevel.Type
                    );
                }
            }
            _state = state;
            yield return null;
        }

        private EState GetState()
        {
            var advData = AppData.Instance.AdventureData;
            var progress = advData.UserData.AdventureUserProgress;
            if (_isBonus)
            {
                if (advData.UserData.SectionList.Count > (_chapterId - 1) &&
                    advData.UserData.SectionList[_chapterId - 1].GotStarCnt >= _levelIdx * 9)
                {
                    var levelData = advData.GetAdventureUserLevelDataDetail(_chapterId,
                        _isBonus ? EAdventureProjectType.APT_Bonus : EAdventureProjectType.APT_Normal, _levelIdx);
                    if (levelData.SimpleData.SuccessCount > 0)
                    {
                        return EState.Pass;
                    }
                    else
                    {
                        return EState.Unlock;
                    }
                }
                else
                {
                    return EState.Lock;
                }
            }
            else
            {
                var curLevelProgress = AdventureData.GetNormalProgress(_chapterId, _levelIdx);
                var completeProgress =
                    AdventureData.GetNormalProgress(progress.CompleteSection, progress.CompleteLevel);
                if (curLevelProgress > completeProgress + 1)
                {
                    return EState.Lock;
                }
                if (curLevelProgress == completeProgress + 1)
                {
                    return EState.Unlock;
                }
                return EState.Pass;
            }
        }

        private void OnClick()
        {
            EAdventureProjectType eAPType =
                _isBonus ? EAdventureProjectType.APT_Bonus : EAdventureProjectType.APT_Normal;
            var param = new SituationAdventureParam();
            param.ProjectType = eAPType;
            param.Section = _chapterId;
            param.Level = _levelIdx;
            var project = AppData.Instance.AdventureData.GetAdvLevelProject(_chapterId, eAPType, _levelIdx);
            if (!_isBonus)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlAdvLvlDetail>(new IntVec3(_chapterId, _levelIdx,
                    _isBonus ? 1 : 0));
            }
            else
            {
                // 奖励关直接进去游戏
                if (GameATools.CheckEnergy(_tableLevel.EnergyCost))
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(
                        this,
                        string.Format("请求进入冒险[{0}]关卡， 第{1}章，第{2}关...", "奖励", _chapterId, _levelIdx));

                    AppData.Instance.AdventureData.PlayAdventureLevel(
                        _chapterId,
                        _levelIdx,
                        eAPType,
                        () =>
                        {
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            GameATools.LocalUseEnergy(_tableLevel.EnergyCost);
                            GameManager.Instance.RequestPlayAdvNormal(project, param);
                            SocialApp.Instance.ChangeToGame();
                        },
                        error => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); }
                    );
                }
            }
        }

        private static float _duration = 2f;
        private static Vector2 _animOffset = Vector2.up * 20;
        private static float _waitTime = 0.2f;

        public void SetTween()
        {
            if (_sequence == null)
            {
                _sequence = DOTween.Sequence()
                    .Append(_cachedView.rectTransform().DOBlendableLocalMoveBy(-_animOffset * 2, _duration))
                    .AppendInterval(_waitTime)
                    .Append(_cachedView.rectTransform().DOBlendableLocalMoveBy(_animOffset * 2, _duration))
                    .AppendInterval(_waitTime)
                    .OnComplete(() => _sequence.Restart()).Pause();
                _cachedView.rectTransform().DOBlendableLocalMoveBy(_animOffset, _duration)
                    .SetDelay(1)
                    .OnComplete(() => _sequence.Play());
            }
        }

        public void StopTween()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
                _sequence = null;
                _cachedView.rectTransform().anchoredPosition = Vector2.zero;
            }
        }

        public void SetIslandImage(Sprite isLandImageSprite)
        {
            _cachedView.IslandImage.sprite = isLandImageSprite;
        }

        public void ClearFriendProgress()
        {
            for (int i = 0; i < _cachedView.FriendHeadImgs.Length; i++)
            {
                _cachedView.FriendHeadImgs[i].SetActiveEx(false);
            }
            for (int i = 0; i < _friends.Length; i++)
            {
                _friends[i] = null;
            }
        }

        public void RefreshFriendProgress(List<UserInfoDetail> friendsDataList)
        {
            if (friendsDataList == null) return;
            friendsDataList.Sort((p, q) =>
            {
                return q.UserInfoSimple.RelationWithMe.Friendliness - p.UserInfoSimple.RelationWithMe.Friendliness;
            });
            for (int i = 0; i < friendsDataList.Count; i++)
            {
                if (i < _cachedView.FriendHeadImgs.Length)
                {
                    _friends[i] = friendsDataList[i];
                    ImageResourceManager.Instance.SetDynamicImage(_cachedView.FriendHeadImgs[i],
                        friendsDataList[i].UserInfoSimple.HeadImgUrl, _cachedView.HeadDefaltTexture);
                    _cachedView.FriendHeadImgs[i].SetActiveEx(true);
                }
            }
        }

        private enum EState
        {
            None,
            Lock,
            Unlock,
            Pass
        }
    }
}