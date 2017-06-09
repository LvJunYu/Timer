using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup (EUIAutoSetupType.Add)]
    public class UICtrlAdvLvlDetail : UICtrlGenericBase<UIViewAdvLvlDetail>
    {
        #region Fields
        private USCtrlAdvLvlDetailInfo _infoPanel;
        private USCtrlAdvLvlDetailRecord _recordPanel;
        private USCtrlAdvLvlDetailRank _rankPanel;
        private List<Record> RecordList = new List<Record>();

        private int _chapterIdx;
        private int _levelIdx;
        private bool _isBonus;

        private int EndDisplayOnRank = 10;
        private int BeginningDisplayOnRank = 0;

        private Game.Table_StandaloneLevel _table;
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen (object parameter)
        {
            base.OnOpen(parameter);
            IntVec3 intVec3Param = (IntVec3)parameter;
            if (intVec3Param == null)
                return;
            _chapterIdx = intVec3Param.x;
            _levelIdx = intVec3Param.y;
            _isBonus = intVec3Param.z == 1;

            var tableChapter = Game.TableManager.Instance.GetStandaloneChapter (_chapterIdx);
            if (null == tableChapter) {
                LogHelper.Error ("Find tableChapter failed, {0}", _chapterIdx);
                SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail> ();
                return;
            }
            int levelId = 0;
            if (_isBonus) {
                if (_levelIdx < tableChapter.BonusLevels.Length) {
                    levelId = tableChapter.BonusLevels [_levelIdx];
                } else {
                    LogHelper.Error ("Find {0}'s bonus level of chapter {1} failed.", _levelIdx, _chapterIdx);
                    SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail> ();
                    return;
                }
            } else {
                if (_levelIdx < tableChapter.NormalLevels.Length) {
                    levelId = tableChapter.NormalLevels [_levelIdx];
                } else {
                    LogHelper.Error ("Find {0}'s normal level of chapter {1} failed.", _levelIdx, _chapterIdx);
                    SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail> ();
                    return;
                }
            }
            _table = Game.TableManager.Instance.GetStandaloneLevel (levelId);
            if (null == _table) {
                LogHelper.Error ("Get table standalonelevel failed. id: {0}", levelId);
                SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail> ();
                return;
            }

            OpenInfoPanel ();
            RefreshRankData();
            RefreshAdventureUserLevelDataDetail();
        }
        
        protected override void OnClose() {
            
            base.OnClose ();
        }
        
        protected override void InitEventListener() {
            base.InitEventListener ();
        }
        
        protected override void OnViewCreated() {
            base.OnViewCreated ();

            _infoPanel = new USCtrlAdvLvlDetailInfo ();
            _infoPanel.Init (_cachedView.InfoPanel);
            _recordPanel= new USCtrlAdvLvlDetailRecord ();
            _recordPanel.Init (_cachedView.RecordPanel);
            _rankPanel = new USCtrlAdvLvlDetailRank ();
            _rankPanel.Init (_cachedView.RankPanel);

            _cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);
            _cachedView.PlayBtn.onClick.AddListener (OnPlayBtn);
            _cachedView.InfoBtn1.onClick.AddListener (OnInfoBtn1);
            _cachedView.RecordBtn1.onClick.AddListener (OnRecordBtn1);
            _cachedView.RankBtn1.onClick.AddListener (OnRankBtn1);
            
        }

        private void RefreshAdventureUserLevelDataDetail()
        {
            LocalUser.Instance.AdventureUserLevelDataDetail.Request(
                LocalUser.Instance.UserGuid,
                _chapterIdx,
                JudgeBonus(),
                _levelIdx,
                null
                , null);

        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        private void RefreshRankData()
        {
            LocalUser.Instance.AdventureLevelRankList.Request(
            _chapterIdx,
            JudgeBonus(),
            _levelIdx,
            BeginningDisplayOnRank,
            EndDisplayOnRank,
            null, null)
            ;

        }

        private EAdventureProjectType JudgeBonus()
        {
            if (_isBonus)
                return EAdventureProjectType.APT_Bonus;
            else if (_isBonus == false)
                return EAdventureProjectType.APT_Normal;
            else return EAdventureProjectType.APT_None;
        }

        private void OpenInfoPanel () {
            Project project;
            if (AppData.Instance.AdventureData.ProjectList.SectionList.Count < _chapterIdx) {
                LogHelper.Error ("No project data of chapter {0}", _chapterIdx);
                SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail> ();
                return;
            } else {
                List<Project> projectList = _isBonus ?
                    AppData.Instance.AdventureData.ProjectList.SectionList [_chapterIdx - 1].BonusProjectList :
                    AppData.Instance.AdventureData.ProjectList.SectionList [_chapterIdx - 1].NormalProjectList;
                if (projectList.Count <= _levelIdx) {
                    LogHelper.Error ("No project data of level in idx {0} in chapter {1}", _levelIdx, _chapterIdx);
                    SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail> ();
                    return;
                } else {
                    project = projectList [_levelIdx];
                }
            }
            _infoPanel.Open (project, _table);
            _recordPanel.Close ();
            _rankPanel.Close ();

            _cachedView.InfoBtn1.gameObject.SetActive (false);
            _cachedView.InfoBtn2.gameObject.SetActive (true);
            _cachedView.RecordBtn1.gameObject.SetActive (true);
            _cachedView.RecordBtn2.gameObject.SetActive (false);
            _cachedView.RankBtn1.gameObject.SetActive (true);
            _cachedView.RankBtn2.gameObject.SetActive (false);
        }
        private void OpenRecordPanel ()
        {
            _infoPanel.Close ();
            _recordPanel.Open ();
            _rankPanel.Close ();

            _cachedView.InfoBtn1.gameObject.SetActive (true);
            _cachedView.InfoBtn2.gameObject.SetActive (false);
            _cachedView.RecordBtn1.gameObject.SetActive (false);
            _cachedView.RecordBtn2.gameObject.SetActive (true);
            _cachedView.RankBtn1.gameObject.SetActive (true);
            _cachedView.RankBtn2.gameObject.SetActive (false);
            _recordPanel.Set();
        }
        private void OpenRankPanel ()
        {
            _infoPanel.Close ();
            _recordPanel.Close ();
            _rankPanel.Open ();

            _cachedView.InfoBtn1.gameObject.SetActive (true);
            _cachedView.InfoBtn2.gameObject.SetActive (false);
            _cachedView.RecordBtn1.gameObject.SetActive (true);
            _cachedView.RecordBtn2.gameObject.SetActive (false);
            _cachedView.RankBtn1.gameObject.SetActive (false);
            _cachedView.RankBtn2.gameObject.SetActive (true);
            _rankPanel.Set(LocalUser.Instance.AdventureLevelRankList.RecordList);
        }
        private void OnCloseBtn () {
            SocialGUIManager.Instance.CloseUI <UICtrlAdvLvlDetail> ();
        }
        private void OnPlayBtn () {
            if (!_isBonus && !GameATools.CheckEnergy (_table.EnergyCost))
                return;
            EAdventureProjectType eAPType = _isBonus ? EAdventureProjectType.APT_Bonus : EAdventureProjectType.APT_Normal;
            //          Debug.Log ("_________________onLevelClicked " + chapterIdx + " " + levelIdx + " isBonus: " + isBonusLevel);


            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (
                this, 
                string.Format ("请求进入冒险[{0}]关卡， 第{1}章，第{2}关...", _isBonus ? "奖励" : "普通", _chapterIdx, _levelIdx));

            AppData.Instance.AdventureData.PlayAdventureLevel (
                _chapterIdx,
                _levelIdx + 1,
                eAPType,
                () => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    // set local energy data
                    GameATools.LocalUseEnergy (_table.EnergyCost);
                },
                (error) => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                }
            );
        }
        private void OnInfoBtn1 () {
            OpenInfoPanel ();
        }
        private void OnRecordBtn1 ()
        {
            OpenRecordPanel ();
        }
        private void OnRankBtn1 ()
        {
            OpenRankPanel ();
        }
        #endregion
    }
}
