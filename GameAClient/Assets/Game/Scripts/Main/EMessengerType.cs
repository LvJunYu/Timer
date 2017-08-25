/********************************************************************
** Filename : EMessengerType
** Author : Dong
** Date : 2015/7/2 16:45:59
** Summary : EMessengerType
***********************************************************************/

namespace GameA
{
    public static partial class EMessengerType
    {
        public static int OnTableInited = SoyEngine.EMessengerType.NextId++;

        public static int OnCommandChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnSelectedItemChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnUndoChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnRedoChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnSelectedItemChangedOnSwitchMode = SoyEngine.EMessengerType.NextId++;

        public static int OnSwitchConnectionChanged = SoyEngine.EMessengerType.NextId++;

		public static int OnModifyEditDeleted = SoyEngine.EMessengerType.NextId++;
		public static int OnModifyEditAdded = SoyEngine.EMessengerType.NextId++;
		public static int OnModifyModified = SoyEngine.EMessengerType.NextId++;
		public static int OnModifyUnitChanged = SoyEngine.EMessengerType.NextId++;
	    
	    public static int OnUnitAddedInEditMode = SoyEngine.EMessengerType.NextId++;
	    public static int OnUnitDeletedInEditMode = SoyEngine.EMessengerType.NextId++;

        public static int OnScreenOperator = SoyEngine.EMessengerType.NextId++;
        public static int OnValidMapRectChanged = SoyEngine.EMessengerType.NextId++;

        public static int OnActorFlip = SoyEngine.EMessengerType.NextId++;

		public static int OnScreenOperatorSuccess = SoyEngine.EMessengerType.NextId++;

        public static int OnEdit = SoyEngine.EMessengerType.NextId++;
        public static int OnPlay = SoyEngine.EMessengerType.NextId++;

        public static int OnLifeChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnKeyChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnWinDataChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnScoreChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnMainPlayerCreated = SoyEngine.EMessengerType.NextId++;

	    public static int GameLog = SoyEngine.EMessengerType.NextId++;
	    public static int GameErrorLog = SoyEngine.EMessengerType.NextId++;

	    public static int GameFinishSuccess  = SoyEngine.EMessengerType.NextId++;
        public static int GameFinishSuccessShowUI = SoyEngine.EMessengerType.NextId++;
        public static int GameFinishFailed = SoyEngine.EMessengerType.NextId++;
        public static int GameFinishFailedShowUI = SoyEngine.EMessengerType.NextId++;
        public static int GameFailedDeadMark = SoyEngine.EMessengerType.NextId++;

        public static int OnMainPlayerDead = SoyEngine.EMessengerType.NextId++;

		public static int OnGameRestart = SoyEngine.EMessengerType.NextId++;

		public static int OnEditCameraOrthoSizeChange = SoyEngine.EMessengerType.NextId++;
		public static int OnEditCameraPosChange = SoyEngine.EMessengerType.NextId++;

		public static int ForceUpdateCameraMaskSize = SoyEngine.EMessengerType.NextId++;
        // 拾取了宝石
        public static int OnGemCollect = SoyEngine.EMessengerType.NextId++;
        // 怪物死了
        public static int OnMonsterDead = SoyEngine.EMessengerType.NextId++;
        // 玩家跳跃
        public static int OnPlayerJump = SoyEngine.EMessengerType.NextId++;
        // 开关被触发
        public static int OnSwitchTriggered = SoyEngine.EMessengerType.NextId++;
        // 玩家使用了传送门
        public static int OnPlayerEnterPortal = SoyEngine.EMessengerType.NextId++;
		public static int OpenGameSetting = SoyEngine.EMessengerType.NextId++;
		public static int OnCloseGameSetting = SoyEngine.EMessengerType.NextId++;

        public static int CaptureGameCover = SoyEngine.EMessengerType.NextId++;

		public static int AfterEditModeStateChange = SoyEngine.EMessengerType.NextId++;

        public static int OnSkillChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnAmmoChanged = SoyEngine.EMessengerType.NextId++;

	    // skill1 怒气 变化，参数1：float 当前怒气，参数2：float 总怒气
	    public static int OnSkill1CDChanged = SoyEngine.EMessengerType.NextId++;
	    // skill2 cd 变化，参数1：float 剩余时间，参数2：float 总CD时长
	    public static int OnSkill2CDChanged = SoyEngine.EMessengerType.NextId++;
	    // skill3 怒气 变化，参数1：float 当前怒气，参数2：float 总怒气
	    public static int OnSkill3CDChanged = SoyEngine.EMessengerType.NextId++;
//	    // 设置 skill2 icon，参数1：string icon名称
//	    public static int SetSkill2Icon = SoyEngine.EMessengerType.NextId++;
//	    // 设置 skill3 icon，参数1：string icon名称
//	    public static int SetSkill3Icon = SoyEngine.EMessengerType.NextId++;
	    /// <summary>
	    /// TableSkill, Slot
	    /// </summary>
	    public static int OnSkillSlotChanged = SoyEngine.EMessengerType.NextId++;

        // 主角加速跑技能cd发生变化
        public static int OnSpeedUpCDChanged = SoyEngine.EMessengerType.NextId++;

        // 主玩家复活
        public static int OnMainPlayerRevive = SoyEngine.EMessengerType.NextId++;

        public static int OnGameLoadError = SoyEngine.EMessengerType.NextId++;

		public static int OnTriggerBulletinBoardEnter = SoyEngine.EMessengerType.NextId++;
		public static int OnTriggerBulletinBoardExit = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnAOISubscribe = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnAOIUnsubscribe = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnDynamicSubscribe = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnDynamicUnsubscribe = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnBgDynamicSubscribe = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnBgDynamicUnsubscribe = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnRespawnPointTrigger = SoyEngine.EMessengerType.NextId++;

	    public static readonly int OnEditorLayerChanged = SoyEngine.EMessengerType.NextId++;
		public static int OnEnterGameLoadingProcess = SoyEngine.EMessengerType.NextId++;
		public static int OnLoadingErrorCloseUI = SoyEngine.EMessengerType.NextId++;
		public static int OnDanmuDataAdded = SoyEngine.EMessengerType.NextId++;

		public static int OnRecordRestart = SoyEngine.EMessengerType.NextId++;

        public static int OnUnitEditChanged = SoyEngine.EMessengerType.NextId++;

		/// <summary>
		/// 改编特效时调用 
		/// </summary>
		public static int OnCurCompositeEditorStateChanged = SoyEngine.EMessengerType.NextId++;

		public static int OnCancelSelectState = SoyEngine.EMessengerType.NextId++;
	    
		public static int OnTrigger = SoyEngine.EMessengerType.NextId++;

	}
}
