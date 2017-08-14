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

        private int _chapterIdx;
        private int _levelIdx;
        private bool _isBonus;

        private int EndDisplayOnRank = 20;
        private int BeginningDisplayOnRank = 0;

        private Game.Table_StandaloneLevel _table;
        #endregion

        #region Properties
        public int ChapterIdx
        {
            get
            {
                return this._chapterIdx;
            }
        }

        public int LevelIdx
        {
            get
            {
                return this._levelIdx;
            }
        }

        public EAdventureProjectType ProjectType
        {
            get
            {
                return this._isBonus ? EAdventureProjectType.APT_Bonus : EAdventureProjectType.APT_Normal;
            }
        }

        public Project Project
        {
            get { return AppData.Instance.AdventureData.GetAdvLevelProject(ChapterIdx, LevelIdx, ProjectType); }
        }
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
            _table = AppData.Instance.AdventureData.GetAdvLevelTable(_chapterIdx, _levelIdx,
                _isBonus ? EAdventureProjectType.APT_Bonus : EAdventureProjectType.APT_Normal);
            if (null == _table) {
                LogHelper.Error ("Get table standalonelevel failed");
                SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail> ();
                return;
            }



            OpenInfoPanel ();
            InitPanel();
            //RefreshRankData();
            //RefreshAdventureUserLevelDataDetail();
        }
        
        protected override void OnClose() {
            
            base.OnClose ();
            //LocalUser.Instance.AdventureUserLevelDataDetail.ClearData();
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

        public void RefreshAdventureUserLevelDataDetail()
        {
            LocalUser.Instance.AdventureUserLevelDataDetail.Request(
                LocalUser.Instance.UserGuid,
                _chapterIdx,
                JudgeBonus(),
                _levelIdx,
                () => { _recordPanel.Set(); }
                , null);

        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.FrontUI2;
        }

        private void RefreshRankData()
        {
            LocalUser.Instance.AdventureLevelRankList.Request(
            _chapterIdx,
            JudgeBonus(),
            _levelIdx,
            BeginningDisplayOnRank,
            EndDisplayOnRank,
                () =>
                {
                    //RefreshAdventureUserLevelDataDetail();
                    _rankPanel.Set(LocalUser.Instance.AdventureLevelRankList.RecordList);
                    _cachedView.FirstName.text = LocalUser.Instance.AdventureLevelRankList.RecordList[0].UserInfo.NickName;
                    //_cachedView.FirstScore.text =string.Format("{0}",) ;
                    //_cachedView.FirstScore.text = "<color=#0000ff><size=60>小明</size></color>送了<color=#0000ff><size=60>小红</size></color>一辆游艇";
                    _cachedView.FirstScore.text =
                        "<color=#84684CFF><size=24>最高得分: </size></color>"
                    +"<color=#FE8300FF><size=28>" + LocalUser.Instance.AdventureLevelRankList.RecordList[0].Score +
                    "</size></color>";
                }
            , null)
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

        private void InitPanel()
        {
            
            Project project;
            if (AppData.Instance.AdventureData.ProjectList.SectionList.Count < _chapterIdx)
            {
                LogHelper.Error("No project data of chapter {0}", _chapterIdx);
                SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail>();
                return;
            }
            else
            {
                List<Project> projectList = _isBonus
                    ? AppData.Instance.AdventureData.ProjectList.SectionList[_chapterIdx - 1].BonusProjectList
                    : AppData.Instance.AdventureData.ProjectList.SectionList[_chapterIdx - 1].NormalProjectList;
                if (projectList.Count <= _levelIdx - 1)
                {
                    LogHelper.Error("No project data of level in idx {0} in chapter {1}", _levelIdx, _chapterIdx);
                    SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail>();
                    return;
                }
                else
                {
                    project = projectList[_levelIdx - 1];
                }

                if (null == project) return;
                ImageResourceManager.Instance.SetDynamicImage(
                    _cachedView.Cover1,
                    project.IconPath,
                    _cachedView.DefaultCover);
                ImageResourceManager.Instance.SetDynamicImage(
                    _cachedView.Cover2,
                    project.IconPath,
                    _cachedView.DefaultCover);
                ImageResourceManager.Instance.SetDynamicImage(
                    _cachedView.Cover3,
                    project.IconPath,
                    _cachedView.DefaultCover);
                RefreshRankData();
              

            }
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
                if (projectList.Count <= _levelIdx - 1) {
                    LogHelper.Error ("No project data of level in idx {0} in chapter {1}", _levelIdx, _chapterIdx);
                    SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail> ();
                    return;
                } else {
                    project = projectList [_levelIdx - 1];
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
            //RefreshRankData();
            RefreshAdventureUserLevelDataDetail();
            //_recordPanel.Set();

            _infoPanel.Close ();
            _recordPanel.Open ();
            _rankPanel.Close ();

            _cachedView.InfoBtn1.gameObject.SetActive (true);
            _cachedView.InfoBtn2.gameObject.SetActive (false);
            _cachedView.RecordBtn1.gameObject.SetActive (false);
            _cachedView.RecordBtn2.gameObject.SetActive (true);
            _cachedView.RankBtn1.gameObject.SetActive (true);
            _cachedView.RankBtn2.gameObject.SetActive (false);
            
        }
        private void OpenRankPanel ()
        {
            _infoPanel.Close ();
            _recordPanel.Close ();
            _rankPanel.Open ();
            RefreshRankData();

            _cachedView.InfoBtn1.gameObject.SetActive (true);
            _cachedView.InfoBtn2.gameObject.SetActive (false);
            _cachedView.RecordBtn1.gameObject.SetActive (true);
            _cachedView.RecordBtn2.gameObject.SetActive (false);
            _cachedView.RankBtn1.gameObject.SetActive (false);
            _cachedView.RankBtn2.gameObject.SetActive (true);
            
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
                _levelIdx,
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
