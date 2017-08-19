using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    [UIAutoSetup]
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

        private Table_StandaloneLevel _table;
        private Project _project;
        private AdventureUserLevelDataDetail _userLevelDataDetail;
        #endregion

        #region Properties
        public int ChapterIdx
        {
            get
            {
                return _chapterIdx;
            }
        }

        public int LevelIdx
        {
            get
            {
                return _levelIdx;
            }
        }

        public EAdventureProjectType ProjectType
        {
            get
            {
                return _isBonus ? EAdventureProjectType.APT_Bonus : EAdventureProjectType.APT_Normal;
            }
        }

        public Project Project
        {
            get { return _project; }
        }
        #endregion

        #region Methods
        protected override void OnOpen (object parameter)
        {
            base.OnOpen(parameter);
            IntVec3 intVec3Param = (IntVec3)parameter;
            _chapterIdx = intVec3Param.x;
            _levelIdx = intVec3Param.y;
            _isBonus = intVec3Param.z == 1;
            _table = AppData.Instance.AdventureData.GetAdvLevelTable(_chapterIdx,
                _isBonus ? EAdventureProjectType.APT_Bonus : EAdventureProjectType.APT_Normal, _levelIdx);
            if (null == _table) {
                LogHelper.Error ("Get table standalonelevel failed");
                SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail> ();
                return;
            }
            _project = AppData.Instance.AdventureData.GetAdvLevelProject(_chapterIdx,
                _isBonus ? EAdventureProjectType.APT_Bonus : EAdventureProjectType.APT_Normal, _levelIdx);
            if (null == _project) {
                LogHelper.Error ("Get Project failed");
                SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail> ();
                return;
            }
            _userLevelDataDetail = AppData.Instance.AdventureData.GetAdventureUserLevelDataDetail(_chapterIdx,
                _isBonus ? EAdventureProjectType.APT_Bonus : EAdventureProjectType.APT_Normal, _levelIdx);

            OpenInfoPanel ();
            InitPanel();
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
            _userLevelDataDetail.Request(
                LocalUser.Instance.UserGuid,
                _chapterIdx,
                JudgeBonus(),
                _levelIdx,
                () => { _recordPanel.Set(_userLevelDataDetail); }
                , null);

        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI2;
        }

        private void RefreshRankData()
        {
            _project.AdventureLevelRankList.Request(
            _chapterIdx,
            JudgeBonus(),
            _levelIdx,
            BeginningDisplayOnRank,
            EndDisplayOnRank,
                () =>
                {
                    //RefreshAdventureUserLevelDataDetail();
                    _rankPanel.Set(_project.AdventureLevelRankList.RecordList);
                    if (_project.AdventureLevelRankList.RecordList.Count > 0)
                    {
                        _cachedView.FirstName.text = _project.AdventureLevelRankList.RecordList[0].UserInfo.NickName;
                        _cachedView.FirstScore.text =
                            "<color=#84684CFF><size=24>最高得分: </size></color>"
                            + "<color=#FE8300FF><size=28>" + _project.AdventureLevelRankList.RecordList[0].Score +
                            "</size></color>";
                    }
                    else
                    {
                        _cachedView.FirstName.text = "--";
                        _cachedView.FirstScore.text =
                            "<color=#84684CFF><size=24>最高得分: </size></color>"
                            + "<color=#FE8300FF><size=28>--</size></color>";
                        
                    }
                }
            , null)
            ;


        }

        private EAdventureProjectType JudgeBonus()
        {
            if (_isBonus)
                return EAdventureProjectType.APT_Bonus;
            if (_isBonus == false)
                return EAdventureProjectType.APT_Normal;
            return EAdventureProjectType.APT_None;
        }

        private void InitPanel()
        {
            
            Project project;
            if (AppData.Instance.AdventureData.ProjectList.SectionList.Count < _chapterIdx)
            {
                LogHelper.Error("No project data of chapter {0}", _chapterIdx);
                SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail>();
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
                project = projectList[_levelIdx - 1];

                if (null == project) return;
                ImageResourceManager.Instance.SetDynamicImage(
                    _cachedView.Cover1,
                    project.IconPath,
                    _cachedView.DefaultCover);
                RefreshRankData();
              

            }
        }

        private void OpenInfoPanel () {
            _infoPanel.Open (_project, _table, _userLevelDataDetail);
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
                error => {
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

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
        }

        private void OnChangeToAppMode()
        {
            if (_isOpen)
            {
                if (!AppData.Instance.AdventureData.LastAdvSuccess)
                {
                    OpenInfoPanel ();
                    InitPanel();
                    return;
                }
                if (_isBonus)
                {
                    SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail>();
                }
                else
                {
                    
                    int nextLevel;
                    if (!AppData.Instance.AdventureData.TryGetNextNormalLevel(_chapterIdx, _levelIdx, out nextLevel))
                    {
                        SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail>();
                    }
                    else
                    {
                        IntVec3 intVec3 = new IntVec3();
                        intVec3.x = _chapterIdx;
                        intVec3.y = nextLevel;
                        intVec3.z = 0;
                        SocialGUIManager.Instance.CloseUI<UICtrlAdvLvlDetail>();
                        CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() =>
                            SocialGUIManager.Instance.OpenUI<UICtrlAdvLvlDetail>(intVec3)));

                    }
                }
            }
        }

        #endregion
    }
}
