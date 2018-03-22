using System;
using System.Collections.Generic;
using UnityEngine;
using SoyEngine;
using NewResourceSolution;
using ResourceManager = NewResourceSolution.ResourcesManager;

namespace GameA.Game
{
	[Serializable]
	public class TableManager : MonoBehaviour
	{
		#region 常量与字段
		private static TableManager _instance;
		public readonly Dictionary<int,Table_State> Table_StateDic = new Dictionary<int, Table_State>();
		public readonly Dictionary<int,Table_Skill> Table_SkillDic = new Dictionary<int, Table_Skill>();
		public readonly Dictionary<int,Table_Reward> Table_RewardDic = new Dictionary<int, Table_Reward>();
		public readonly Dictionary<int,Table_FashionShop> Table_FashionShopDic = new Dictionary<int, Table_FashionShop>();
		public readonly Dictionary<int,Table_Equipment> Table_EquipmentDic = new Dictionary<int, Table_Equipment>();
		public readonly Dictionary<int,Table_EquipmentLevel> Table_EquipmentLevelDic = new Dictionary<int, Table_EquipmentLevel>();
		public readonly Dictionary<int,Table_Trap> Table_TrapDic = new Dictionary<int, Table_Trap>();
		public readonly Dictionary<int,Table_Unit> Table_UnitDic = new Dictionary<int, Table_Unit>();
		public readonly Dictionary<int,Table_CharacterUpgrade> Table_CharacterUpgradeDic = new Dictionary<int, Table_CharacterUpgrade>();
		public readonly Dictionary<int,Table_StarRequire> Table_StarRequireDic = new Dictionary<int, Table_StarRequire>();
		public readonly Dictionary<int,Table_StandaloneChapter> Table_StandaloneChapterDic = new Dictionary<int, Table_StandaloneChapter>();
		public readonly Dictionary<int,Table_StandaloneLevel> Table_StandaloneLevelDic = new Dictionary<int, Table_StandaloneLevel>();
		public readonly Dictionary<int,Table_Achievement> Table_AchievementDic = new Dictionary<int, Table_Achievement>();
		public readonly Dictionary<int,Table_FashionUnit> Table_FashionUnitDic = new Dictionary<int, Table_FashionUnit>();
		public readonly Dictionary<int,Table_HonorReport> Table_HonorReportDic = new Dictionary<int, Table_HonorReport>();
		public readonly Dictionary<int,Table_TreasureMap> Table_TreasureMapDic = new Dictionary<int, Table_TreasureMap>();
		public readonly Dictionary<int,Table_Turntable> Table_TurntableDic = new Dictionary<int, Table_Turntable>();
		public readonly Dictionary<int,Table_QQHallGrowAward> Table_QQHallGrowAwardDic = new Dictionary<int, Table_QQHallGrowAward>();
		public readonly Dictionary<int,Table_FashionCoupon> Table_FashionCouponDic = new Dictionary<int, Table_FashionCoupon>();
		public readonly Dictionary<int,Table_Background> Table_BackgroundDic = new Dictionary<int, Table_Background>();
		public readonly Dictionary<int,Table_Decorate> Table_DecorateDic = new Dictionary<int, Table_Decorate>();
		public readonly Dictionary<int,Table_DiamondShop> Table_DiamondShopDic = new Dictionary<int, Table_DiamondShop>();
		public readonly Dictionary<int,Table_PuzzleSummon> Table_PuzzleSummonDic = new Dictionary<int, Table_PuzzleSummon>();
		public readonly Dictionary<int,Table_Puzzle> Table_PuzzleDic = new Dictionary<int, Table_Puzzle>();
		public readonly Dictionary<int,Table_PuzzleUpgrade> Table_PuzzleUpgradeDic = new Dictionary<int, Table_PuzzleUpgrade>();
		public readonly Dictionary<int,Table_PuzzleSlot> Table_PuzzleSlotDic = new Dictionary<int, Table_PuzzleSlot>();
		public readonly Dictionary<int,Table_AvatarStruct> Table_AvatarStructDic = new Dictionary<int, Table_AvatarStruct>();
		public readonly Dictionary<int,Table_Matrix> Table_MatrixDic = new Dictionary<int, Table_Matrix>();
		public readonly Dictionary<int,Table_AvatarSlotName> Table_AvatarSlotNameDic = new Dictionary<int, Table_AvatarSlotName>();
		public readonly Dictionary<int,Table_Morph> Table_MorphDic = new Dictionary<int, Table_Morph>();
		public readonly Dictionary<int,Table_PlayerLvToModifyLimit> Table_PlayerLvToModifyLimitDic = new Dictionary<int, Table_PlayerLvToModifyLimit>();
		public readonly Dictionary<int,Table_PlayerLvToExp> Table_PlayerLvToExpDic = new Dictionary<int, Table_PlayerLvToExp>();
		public readonly Dictionary<int,Table_ModifyReward> Table_ModifyRewardDic = new Dictionary<int, Table_ModifyReward>();
		public readonly Dictionary<int,Table_ProgressUnlock> Table_ProgressUnlockDic = new Dictionary<int, Table_ProgressUnlock>();
		public readonly Dictionary<int,Table_BoostItem> Table_BoostItemDic = new Dictionary<int, Table_BoostItem>();
		public readonly Dictionary<int,Table_NpcTaskTargetColltion> Table_NpcTaskTargetColltionDic = new Dictionary<int, Table_NpcTaskTargetColltion>();
		public readonly Dictionary<int,Table_NpcTaskAward> Table_NpcTaskAwardDic = new Dictionary<int, Table_NpcTaskAward>();
		public readonly Dictionary<int,Table_NpcTaskTargetKill> Table_NpcTaskTargetKillDic = new Dictionary<int, Table_NpcTaskTargetKill>();
		public readonly Dictionary<int,Table_NpcDefaultDia> Table_NpcDefaultDiaDic = new Dictionary<int, Table_NpcDefaultDia>();
		public readonly Dictionary<int,Table_WorkShopNumberOfSlot> Table_WorkShopNumberOfSlotDic = new Dictionary<int, Table_WorkShopNumberOfSlot>();
		[SerializeField] private Table_State[] _tableStates;
		[SerializeField] private Table_Skill[] _tableSkills;
		[SerializeField] private Table_Reward[] _tableRewards;
		[SerializeField] private Table_FashionShop[] _tableFashionShops;
		[SerializeField] private Table_Equipment[] _tableEquipments;
		[SerializeField] private Table_EquipmentLevel[] _tableEquipmentLevels;
		[SerializeField] private Table_Trap[] _tableTraps;
		[SerializeField] private Table_Unit[] _tableUnits;
		[SerializeField] private Table_CharacterUpgrade[] _tableCharacterUpgrades;
		[SerializeField] private Table_StarRequire[] _tableStarRequires;
		[SerializeField] private Table_StandaloneChapter[] _tableStandaloneChapters;
		[SerializeField] private Table_StandaloneLevel[] _tableStandaloneLevels;
		[SerializeField] private Table_Achievement[] _tableAchievements;
		[SerializeField] private Table_FashionUnit[] _tableFashionUnits;
		[SerializeField] private Table_HonorReport[] _tableHonorReports;
		[SerializeField] private Table_TreasureMap[] _tableTreasureMaps;
		[SerializeField] private Table_Turntable[] _tableTurntables;
		[SerializeField] private Table_QQHallGrowAward[] _tableQQHallGrowAwards;
		[SerializeField] private Table_FashionCoupon[] _tableFashionCoupons;
		[SerializeField] private Table_Background[] _tableBackgrounds;
		[SerializeField] private Table_Decorate[] _tableDecorates;
		[SerializeField] private Table_DiamondShop[] _tableDiamondShops;
		[SerializeField] private Table_PuzzleSummon[] _tablePuzzleSummons;
		[SerializeField] private Table_Puzzle[] _tablePuzzles;
		[SerializeField] private Table_PuzzleUpgrade[] _tablePuzzleUpgrades;
		[SerializeField] private Table_PuzzleSlot[] _tablePuzzleSlots;
		[SerializeField] private Table_AvatarStruct[] _tableAvatarStructs;
		[SerializeField] private Table_Matrix[] _tableMatrixs;
		[SerializeField] private Table_AvatarSlotName[] _tableAvatarSlotNames;
		[SerializeField] private Table_Morph[] _tableMorphs;
		[SerializeField] private Table_PlayerLvToModifyLimit[] _tablePlayerLvToModifyLimits;
		[SerializeField] private Table_PlayerLvToExp[] _tablePlayerLvToExps;
		[SerializeField] private Table_ModifyReward[] _tableModifyRewards;
		[SerializeField] private Table_ProgressUnlock[] _tableProgressUnlocks;
		[SerializeField] private Table_BoostItem[] _tableBoostItems;
		[SerializeField] private Table_NpcTaskTargetColltion[] _tableNpcTaskTargetColltions;
		[SerializeField] private Table_NpcTaskAward[] _tableNpcTaskAwards;
		[SerializeField] private Table_NpcTaskTargetKill[] _tableNpcTaskTargetKills;
		[SerializeField] private Table_NpcDefaultDia[] _tableNpcDefaultDias;
		[SerializeField] private Table_WorkShopNumberOfSlot[] _tableWorkShopNumberOfSlots;

		#endregion
		#region 属性
		public static TableManager Instance
		{
			get { return _instance; }
		}
		#endregion
		#region 方法
		private void Awake()
		{
			_instance = this;
		}
		public void Init()
		{
			string StateJsonStr = JoyResManager.Instance.GetJson ("State", (int) EResScenary.TableAsset);
            _tableStates = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_State[]>(StateJsonStr);
			string SkillJsonStr = JoyResManager.Instance.GetJson ("Skill", (int) EResScenary.TableAsset);
            _tableSkills = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Skill[]>(SkillJsonStr);
			string RewardJsonStr = JoyResManager.Instance.GetJson ("Reward", (int) EResScenary.TableAsset);
            _tableRewards = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Reward[]>(RewardJsonStr);
			string FashionShopJsonStr = JoyResManager.Instance.GetJson ("FashionShop", (int) EResScenary.TableAsset);
            _tableFashionShops = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_FashionShop[]>(FashionShopJsonStr);
			string EquipmentJsonStr = JoyResManager.Instance.GetJson ("Equipment", (int) EResScenary.TableAsset);
            _tableEquipments = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Equipment[]>(EquipmentJsonStr);
			string EquipmentLevelJsonStr = JoyResManager.Instance.GetJson ("EquipmentLevel", (int) EResScenary.TableAsset);
            _tableEquipmentLevels = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_EquipmentLevel[]>(EquipmentLevelJsonStr);
			string TrapJsonStr = JoyResManager.Instance.GetJson ("Trap", (int) EResScenary.TableAsset);
            _tableTraps = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Trap[]>(TrapJsonStr);
			string UnitJsonStr = JoyResManager.Instance.GetJson ("Unit", (int) EResScenary.TableAsset);
            _tableUnits = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Unit[]>(UnitJsonStr);
			string CharacterUpgradeJsonStr = JoyResManager.Instance.GetJson ("CharacterUpgrade", (int) EResScenary.TableAsset);
            _tableCharacterUpgrades = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_CharacterUpgrade[]>(CharacterUpgradeJsonStr);
			string StarRequireJsonStr = JoyResManager.Instance.GetJson ("StarRequire", (int) EResScenary.TableAsset);
            _tableStarRequires = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_StarRequire[]>(StarRequireJsonStr);
			string StandaloneChapterJsonStr = JoyResManager.Instance.GetJson ("StandaloneChapter", (int) EResScenary.TableAsset);
            _tableStandaloneChapters = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_StandaloneChapter[]>(StandaloneChapterJsonStr);
			string StandaloneLevelJsonStr = JoyResManager.Instance.GetJson ("StandaloneLevel", (int) EResScenary.TableAsset);
            _tableStandaloneLevels = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_StandaloneLevel[]>(StandaloneLevelJsonStr);
			string AchievementJsonStr = JoyResManager.Instance.GetJson ("Achievement", (int) EResScenary.TableAsset);
            _tableAchievements = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Achievement[]>(AchievementJsonStr);
			string FashionUnitJsonStr = JoyResManager.Instance.GetJson ("FashionUnit", (int) EResScenary.TableAsset);
            _tableFashionUnits = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_FashionUnit[]>(FashionUnitJsonStr);
			string HonorReportJsonStr = JoyResManager.Instance.GetJson ("HonorReport", (int) EResScenary.TableAsset);
            _tableHonorReports = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_HonorReport[]>(HonorReportJsonStr);
			string TreasureMapJsonStr = JoyResManager.Instance.GetJson ("TreasureMap", (int) EResScenary.TableAsset);
            _tableTreasureMaps = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_TreasureMap[]>(TreasureMapJsonStr);
			string TurntableJsonStr = JoyResManager.Instance.GetJson ("Turntable", (int) EResScenary.TableAsset);
            _tableTurntables = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Turntable[]>(TurntableJsonStr);
			string QQHallGrowAwardJsonStr = JoyResManager.Instance.GetJson ("QQHallGrowAward", (int) EResScenary.TableAsset);
            _tableQQHallGrowAwards = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_QQHallGrowAward[]>(QQHallGrowAwardJsonStr);
			string FashionCouponJsonStr = JoyResManager.Instance.GetJson ("FashionCoupon", (int) EResScenary.TableAsset);
            _tableFashionCoupons = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_FashionCoupon[]>(FashionCouponJsonStr);
			string BackgroundJsonStr = JoyResManager.Instance.GetJson ("Background", (int) EResScenary.TableAsset);
            _tableBackgrounds = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Background[]>(BackgroundJsonStr);
			string DecorateJsonStr = JoyResManager.Instance.GetJson ("Decorate", (int) EResScenary.TableAsset);
            _tableDecorates = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Decorate[]>(DecorateJsonStr);
			string DiamondShopJsonStr = JoyResManager.Instance.GetJson ("DiamondShop", (int) EResScenary.TableAsset);
            _tableDiamondShops = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_DiamondShop[]>(DiamondShopJsonStr);
			string PuzzleSummonJsonStr = JoyResManager.Instance.GetJson ("PuzzleSummon", (int) EResScenary.TableAsset);
            _tablePuzzleSummons = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_PuzzleSummon[]>(PuzzleSummonJsonStr);
			string PuzzleJsonStr = JoyResManager.Instance.GetJson ("Puzzle", (int) EResScenary.TableAsset);
            _tablePuzzles = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Puzzle[]>(PuzzleJsonStr);
			string PuzzleUpgradeJsonStr = JoyResManager.Instance.GetJson ("PuzzleUpgrade", (int) EResScenary.TableAsset);
            _tablePuzzleUpgrades = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_PuzzleUpgrade[]>(PuzzleUpgradeJsonStr);
			string PuzzleSlotJsonStr = JoyResManager.Instance.GetJson ("PuzzleSlot", (int) EResScenary.TableAsset);
            _tablePuzzleSlots = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_PuzzleSlot[]>(PuzzleSlotJsonStr);
			string AvatarStructJsonStr = JoyResManager.Instance.GetJson ("AvatarStruct", (int) EResScenary.TableAsset);
            _tableAvatarStructs = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_AvatarStruct[]>(AvatarStructJsonStr);
			string MatrixJsonStr = JoyResManager.Instance.GetJson ("Matrix", (int) EResScenary.TableAsset);
            _tableMatrixs = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Matrix[]>(MatrixJsonStr);
			string AvatarSlotNameJsonStr = JoyResManager.Instance.GetJson ("AvatarSlotName", (int) EResScenary.TableAsset);
            _tableAvatarSlotNames = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_AvatarSlotName[]>(AvatarSlotNameJsonStr);
			string MorphJsonStr = JoyResManager.Instance.GetJson ("Morph", (int) EResScenary.TableAsset);
            _tableMorphs = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Morph[]>(MorphJsonStr);
			string PlayerLvToModifyLimitJsonStr = JoyResManager.Instance.GetJson ("PlayerLvToModifyLimit", (int) EResScenary.TableAsset);
            _tablePlayerLvToModifyLimits = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_PlayerLvToModifyLimit[]>(PlayerLvToModifyLimitJsonStr);
			string PlayerLvToExpJsonStr = JoyResManager.Instance.GetJson ("PlayerLvToExp", (int) EResScenary.TableAsset);
            _tablePlayerLvToExps = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_PlayerLvToExp[]>(PlayerLvToExpJsonStr);
			string ModifyRewardJsonStr = JoyResManager.Instance.GetJson ("ModifyReward", (int) EResScenary.TableAsset);
            _tableModifyRewards = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_ModifyReward[]>(ModifyRewardJsonStr);
			string ProgressUnlockJsonStr = JoyResManager.Instance.GetJson ("ProgressUnlock", (int) EResScenary.TableAsset);
            _tableProgressUnlocks = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_ProgressUnlock[]>(ProgressUnlockJsonStr);
			string BoostItemJsonStr = JoyResManager.Instance.GetJson ("BoostItem", (int) EResScenary.TableAsset);
            _tableBoostItems = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_BoostItem[]>(BoostItemJsonStr);
			string NpcTaskTargetColltionJsonStr = JoyResManager.Instance.GetJson ("NpcTaskTargetColltion", (int) EResScenary.TableAsset);
            _tableNpcTaskTargetColltions = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_NpcTaskTargetColltion[]>(NpcTaskTargetColltionJsonStr);
			string NpcTaskAwardJsonStr = JoyResManager.Instance.GetJson ("NpcTaskAward", (int) EResScenary.TableAsset);
            _tableNpcTaskAwards = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_NpcTaskAward[]>(NpcTaskAwardJsonStr);
			string NpcTaskTargetKillJsonStr = JoyResManager.Instance.GetJson ("NpcTaskTargetKill", (int) EResScenary.TableAsset);
            _tableNpcTaskTargetKills = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_NpcTaskTargetKill[]>(NpcTaskTargetKillJsonStr);
			string NpcDefaultDiaJsonStr = JoyResManager.Instance.GetJson ("NpcDefaultDia", (int) EResScenary.TableAsset);
            _tableNpcDefaultDias = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_NpcDefaultDia[]>(NpcDefaultDiaJsonStr);
			string WorkShopNumberOfSlotJsonStr = JoyResManager.Instance.GetJson ("WorkShopNumberOfSlot", (int) EResScenary.TableAsset);
            _tableWorkShopNumberOfSlots = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_WorkShopNumberOfSlot[]>(WorkShopNumberOfSlotJsonStr);
			JoyResManager.Instance.UnloadScenary((int) EResScenary.TableAsset);
			for (int i = 0; i < _tableStates.Length; i++)
			{
				if (!Table_StateDic.ContainsKey(_tableStates[i].Id))
				{
					Table_StateDic.Add(_tableStates[i].Id,_tableStates[i]);
				}
				else
				{
					LogHelper.Warning("_tableStates table.Id {0} is duplicated!", _tableStates[i].Id);
				}
			}
			for (int i = 0; i < _tableSkills.Length; i++)
			{
				if (!Table_SkillDic.ContainsKey(_tableSkills[i].Id))
				{
					Table_SkillDic.Add(_tableSkills[i].Id,_tableSkills[i]);
				}
				else
				{
					LogHelper.Warning("_tableSkills table.Id {0} is duplicated!", _tableSkills[i].Id);
				}
			}
			for (int i = 0; i < _tableRewards.Length; i++)
			{
				if (!Table_RewardDic.ContainsKey(_tableRewards[i].Id))
				{
					Table_RewardDic.Add(_tableRewards[i].Id,_tableRewards[i]);
				}
				else
				{
					LogHelper.Warning("_tableRewards table.Id {0} is duplicated!", _tableRewards[i].Id);
				}
			}
			for (int i = 0; i < _tableFashionShops.Length; i++)
			{
				if (!Table_FashionShopDic.ContainsKey(_tableFashionShops[i].Id))
				{
					Table_FashionShopDic.Add(_tableFashionShops[i].Id,_tableFashionShops[i]);
				}
				else
				{
					LogHelper.Warning("_tableFashionShops table.Id {0} is duplicated!", _tableFashionShops[i].Id);
				}
			}
			for (int i = 0; i < _tableEquipments.Length; i++)
			{
				if (!Table_EquipmentDic.ContainsKey(_tableEquipments[i].Id))
				{
					Table_EquipmentDic.Add(_tableEquipments[i].Id,_tableEquipments[i]);
				}
				else
				{
					LogHelper.Warning("_tableEquipments table.Id {0} is duplicated!", _tableEquipments[i].Id);
				}
			}
			for (int i = 0; i < _tableEquipmentLevels.Length; i++)
			{
				if (!Table_EquipmentLevelDic.ContainsKey(_tableEquipmentLevels[i].Id))
				{
					Table_EquipmentLevelDic.Add(_tableEquipmentLevels[i].Id,_tableEquipmentLevels[i]);
				}
				else
				{
					LogHelper.Warning("_tableEquipmentLevels table.Id {0} is duplicated!", _tableEquipmentLevels[i].Id);
				}
			}
			for (int i = 0; i < _tableTraps.Length; i++)
			{
				if (!Table_TrapDic.ContainsKey(_tableTraps[i].Id))
				{
					Table_TrapDic.Add(_tableTraps[i].Id,_tableTraps[i]);
				}
				else
				{
					LogHelper.Warning("_tableTraps table.Id {0} is duplicated!", _tableTraps[i].Id);
				}
			}
			for (int i = 0; i < _tableUnits.Length; i++)
			{
				if (!Table_UnitDic.ContainsKey(_tableUnits[i].Id))
				{
					Table_UnitDic.Add(_tableUnits[i].Id,_tableUnits[i]);
				}
				else
				{
					LogHelper.Warning("_tableUnits table.Id {0} is duplicated!", _tableUnits[i].Id);
				}
			}
			for (int i = 0; i < _tableCharacterUpgrades.Length; i++)
			{
				if (!Table_CharacterUpgradeDic.ContainsKey(_tableCharacterUpgrades[i].Id))
				{
					Table_CharacterUpgradeDic.Add(_tableCharacterUpgrades[i].Id,_tableCharacterUpgrades[i]);
				}
				else
				{
					LogHelper.Warning("_tableCharacterUpgrades table.Id {0} is duplicated!", _tableCharacterUpgrades[i].Id);
				}
			}
			for (int i = 0; i < _tableStarRequires.Length; i++)
			{
				if (!Table_StarRequireDic.ContainsKey(_tableStarRequires[i].Id))
				{
					Table_StarRequireDic.Add(_tableStarRequires[i].Id,_tableStarRequires[i]);
				}
				else
				{
					LogHelper.Warning("_tableStarRequires table.Id {0} is duplicated!", _tableStarRequires[i].Id);
				}
			}
			for (int i = 0; i < _tableStandaloneChapters.Length; i++)
			{
				if (!Table_StandaloneChapterDic.ContainsKey(_tableStandaloneChapters[i].Id))
				{
					Table_StandaloneChapterDic.Add(_tableStandaloneChapters[i].Id,_tableStandaloneChapters[i]);
				}
				else
				{
					LogHelper.Warning("_tableStandaloneChapters table.Id {0} is duplicated!", _tableStandaloneChapters[i].Id);
				}
			}
			for (int i = 0; i < _tableStandaloneLevels.Length; i++)
			{
				if (!Table_StandaloneLevelDic.ContainsKey(_tableStandaloneLevels[i].Id))
				{
					Table_StandaloneLevelDic.Add(_tableStandaloneLevels[i].Id,_tableStandaloneLevels[i]);
				}
				else
				{
					LogHelper.Warning("_tableStandaloneLevels table.Id {0} is duplicated!", _tableStandaloneLevels[i].Id);
				}
			}
			for (int i = 0; i < _tableAchievements.Length; i++)
			{
				if (!Table_AchievementDic.ContainsKey(_tableAchievements[i].Id))
				{
					Table_AchievementDic.Add(_tableAchievements[i].Id,_tableAchievements[i]);
				}
				else
				{
					LogHelper.Warning("_tableAchievements table.Id {0} is duplicated!", _tableAchievements[i].Id);
				}
			}
			for (int i = 0; i < _tableFashionUnits.Length; i++)
			{
				if (!Table_FashionUnitDic.ContainsKey(_tableFashionUnits[i].Id))
				{
					Table_FashionUnitDic.Add(_tableFashionUnits[i].Id,_tableFashionUnits[i]);
				}
				else
				{
					LogHelper.Warning("_tableFashionUnits table.Id {0} is duplicated!", _tableFashionUnits[i].Id);
				}
			}
			for (int i = 0; i < _tableHonorReports.Length; i++)
			{
				if (!Table_HonorReportDic.ContainsKey(_tableHonorReports[i].Id))
				{
					Table_HonorReportDic.Add(_tableHonorReports[i].Id,_tableHonorReports[i]);
				}
				else
				{
					LogHelper.Warning("_tableHonorReports table.Id {0} is duplicated!", _tableHonorReports[i].Id);
				}
			}
			for (int i = 0; i < _tableTreasureMaps.Length; i++)
			{
				if (!Table_TreasureMapDic.ContainsKey(_tableTreasureMaps[i].Id))
				{
					Table_TreasureMapDic.Add(_tableTreasureMaps[i].Id,_tableTreasureMaps[i]);
				}
				else
				{
					LogHelper.Warning("_tableTreasureMaps table.Id {0} is duplicated!", _tableTreasureMaps[i].Id);
				}
			}
			for (int i = 0; i < _tableTurntables.Length; i++)
			{
				if (!Table_TurntableDic.ContainsKey(_tableTurntables[i].Id))
				{
					Table_TurntableDic.Add(_tableTurntables[i].Id,_tableTurntables[i]);
				}
				else
				{
					LogHelper.Warning("_tableTurntables table.Id {0} is duplicated!", _tableTurntables[i].Id);
				}
			}
			for (int i = 0; i < _tableQQHallGrowAwards.Length; i++)
			{
				if (!Table_QQHallGrowAwardDic.ContainsKey(_tableQQHallGrowAwards[i].Id))
				{
					Table_QQHallGrowAwardDic.Add(_tableQQHallGrowAwards[i].Id,_tableQQHallGrowAwards[i]);
				}
				else
				{
					LogHelper.Warning("_tableQQHallGrowAwards table.Id {0} is duplicated!", _tableQQHallGrowAwards[i].Id);
				}
			}
			for (int i = 0; i < _tableFashionCoupons.Length; i++)
			{
				if (!Table_FashionCouponDic.ContainsKey(_tableFashionCoupons[i].Id))
				{
					Table_FashionCouponDic.Add(_tableFashionCoupons[i].Id,_tableFashionCoupons[i]);
				}
				else
				{
					LogHelper.Warning("_tableFashionCoupons table.Id {0} is duplicated!", _tableFashionCoupons[i].Id);
				}
			}
			for (int i = 0; i < _tableBackgrounds.Length; i++)
			{
				if (!Table_BackgroundDic.ContainsKey(_tableBackgrounds[i].Id))
				{
					Table_BackgroundDic.Add(_tableBackgrounds[i].Id,_tableBackgrounds[i]);
				}
				else
				{
					LogHelper.Warning("_tableBackgrounds table.Id {0} is duplicated!", _tableBackgrounds[i].Id);
				}
			}
			for (int i = 0; i < _tableDecorates.Length; i++)
			{
				if (!Table_DecorateDic.ContainsKey(_tableDecorates[i].Id))
				{
					Table_DecorateDic.Add(_tableDecorates[i].Id,_tableDecorates[i]);
				}
				else
				{
					LogHelper.Warning("_tableDecorates table.Id {0} is duplicated!", _tableDecorates[i].Id);
				}
			}
			for (int i = 0; i < _tableDiamondShops.Length; i++)
			{
				if (!Table_DiamondShopDic.ContainsKey(_tableDiamondShops[i].Id))
				{
					Table_DiamondShopDic.Add(_tableDiamondShops[i].Id,_tableDiamondShops[i]);
				}
				else
				{
					LogHelper.Warning("_tableDiamondShops table.Id {0} is duplicated!", _tableDiamondShops[i].Id);
				}
			}
			for (int i = 0; i < _tablePuzzleSummons.Length; i++)
			{
				if (!Table_PuzzleSummonDic.ContainsKey(_tablePuzzleSummons[i].Id))
				{
					Table_PuzzleSummonDic.Add(_tablePuzzleSummons[i].Id,_tablePuzzleSummons[i]);
				}
				else
				{
					LogHelper.Warning("_tablePuzzleSummons table.Id {0} is duplicated!", _tablePuzzleSummons[i].Id);
				}
			}
			for (int i = 0; i < _tablePuzzles.Length; i++)
			{
				if (!Table_PuzzleDic.ContainsKey(_tablePuzzles[i].Id))
				{
					Table_PuzzleDic.Add(_tablePuzzles[i].Id,_tablePuzzles[i]);
				}
				else
				{
					LogHelper.Warning("_tablePuzzles table.Id {0} is duplicated!", _tablePuzzles[i].Id);
				}
			}
			for (int i = 0; i < _tablePuzzleUpgrades.Length; i++)
			{
				if (!Table_PuzzleUpgradeDic.ContainsKey(_tablePuzzleUpgrades[i].Id))
				{
					Table_PuzzleUpgradeDic.Add(_tablePuzzleUpgrades[i].Id,_tablePuzzleUpgrades[i]);
				}
				else
				{
					LogHelper.Warning("_tablePuzzleUpgrades table.Id {0} is duplicated!", _tablePuzzleUpgrades[i].Id);
				}
			}
			for (int i = 0; i < _tablePuzzleSlots.Length; i++)
			{
				if (!Table_PuzzleSlotDic.ContainsKey(_tablePuzzleSlots[i].Id))
				{
					Table_PuzzleSlotDic.Add(_tablePuzzleSlots[i].Id,_tablePuzzleSlots[i]);
				}
				else
				{
					LogHelper.Warning("_tablePuzzleSlots table.Id {0} is duplicated!", _tablePuzzleSlots[i].Id);
				}
			}
			for (int i = 0; i < _tableAvatarStructs.Length; i++)
			{
				if (!Table_AvatarStructDic.ContainsKey(_tableAvatarStructs[i].Id))
				{
					Table_AvatarStructDic.Add(_tableAvatarStructs[i].Id,_tableAvatarStructs[i]);
				}
				else
				{
					LogHelper.Warning("_tableAvatarStructs table.Id {0} is duplicated!", _tableAvatarStructs[i].Id);
				}
			}
			for (int i = 0; i < _tableMatrixs.Length; i++)
			{
				if (!Table_MatrixDic.ContainsKey(_tableMatrixs[i].Id))
				{
					Table_MatrixDic.Add(_tableMatrixs[i].Id,_tableMatrixs[i]);
				}
				else
				{
					LogHelper.Warning("_tableMatrixs table.Id {0} is duplicated!", _tableMatrixs[i].Id);
				}
			}
			for (int i = 0; i < _tableAvatarSlotNames.Length; i++)
			{
				if (!Table_AvatarSlotNameDic.ContainsKey(_tableAvatarSlotNames[i].Id))
				{
					Table_AvatarSlotNameDic.Add(_tableAvatarSlotNames[i].Id,_tableAvatarSlotNames[i]);
				}
				else
				{
					LogHelper.Warning("_tableAvatarSlotNames table.Id {0} is duplicated!", _tableAvatarSlotNames[i].Id);
				}
			}
			for (int i = 0; i < _tableMorphs.Length; i++)
			{
				if (!Table_MorphDic.ContainsKey(_tableMorphs[i].Id))
				{
					Table_MorphDic.Add(_tableMorphs[i].Id,_tableMorphs[i]);
				}
				else
				{
					LogHelper.Warning("_tableMorphs table.Id {0} is duplicated!", _tableMorphs[i].Id);
				}
			}
			for (int i = 0; i < _tablePlayerLvToModifyLimits.Length; i++)
			{
				if (!Table_PlayerLvToModifyLimitDic.ContainsKey(_tablePlayerLvToModifyLimits[i].Id))
				{
					Table_PlayerLvToModifyLimitDic.Add(_tablePlayerLvToModifyLimits[i].Id,_tablePlayerLvToModifyLimits[i]);
				}
				else
				{
					LogHelper.Warning("_tablePlayerLvToModifyLimits table.Id {0} is duplicated!", _tablePlayerLvToModifyLimits[i].Id);
				}
			}
			for (int i = 0; i < _tablePlayerLvToExps.Length; i++)
			{
				if (!Table_PlayerLvToExpDic.ContainsKey(_tablePlayerLvToExps[i].Id))
				{
					Table_PlayerLvToExpDic.Add(_tablePlayerLvToExps[i].Id,_tablePlayerLvToExps[i]);
				}
				else
				{
					LogHelper.Warning("_tablePlayerLvToExps table.Id {0} is duplicated!", _tablePlayerLvToExps[i].Id);
				}
			}
			for (int i = 0; i < _tableModifyRewards.Length; i++)
			{
				if (!Table_ModifyRewardDic.ContainsKey(_tableModifyRewards[i].Id))
				{
					Table_ModifyRewardDic.Add(_tableModifyRewards[i].Id,_tableModifyRewards[i]);
				}
				else
				{
					LogHelper.Warning("_tableModifyRewards table.Id {0} is duplicated!", _tableModifyRewards[i].Id);
				}
			}
			for (int i = 0; i < _tableProgressUnlocks.Length; i++)
			{
				if (!Table_ProgressUnlockDic.ContainsKey(_tableProgressUnlocks[i].Id))
				{
					Table_ProgressUnlockDic.Add(_tableProgressUnlocks[i].Id,_tableProgressUnlocks[i]);
				}
				else
				{
					LogHelper.Warning("_tableProgressUnlocks table.Id {0} is duplicated!", _tableProgressUnlocks[i].Id);
				}
			}
			for (int i = 0; i < _tableBoostItems.Length; i++)
			{
				if (!Table_BoostItemDic.ContainsKey(_tableBoostItems[i].Id))
				{
					Table_BoostItemDic.Add(_tableBoostItems[i].Id,_tableBoostItems[i]);
				}
				else
				{
					LogHelper.Warning("_tableBoostItems table.Id {0} is duplicated!", _tableBoostItems[i].Id);
				}
			}
			for (int i = 0; i < _tableNpcTaskTargetColltions.Length; i++)
			{
				if (!Table_NpcTaskTargetColltionDic.ContainsKey(_tableNpcTaskTargetColltions[i].Id))
				{
					Table_NpcTaskTargetColltionDic.Add(_tableNpcTaskTargetColltions[i].Id,_tableNpcTaskTargetColltions[i]);
				}
				else
				{
					LogHelper.Warning("_tableNpcTaskTargetColltions table.Id {0} is duplicated!", _tableNpcTaskTargetColltions[i].Id);
				}
			}
			for (int i = 0; i < _tableNpcTaskAwards.Length; i++)
			{
				if (!Table_NpcTaskAwardDic.ContainsKey(_tableNpcTaskAwards[i].Id))
				{
					Table_NpcTaskAwardDic.Add(_tableNpcTaskAwards[i].Id,_tableNpcTaskAwards[i]);
				}
				else
				{
					LogHelper.Warning("_tableNpcTaskAwards table.Id {0} is duplicated!", _tableNpcTaskAwards[i].Id);
				}
			}
			for (int i = 0; i < _tableNpcTaskTargetKills.Length; i++)
			{
				if (!Table_NpcTaskTargetKillDic.ContainsKey(_tableNpcTaskTargetKills[i].Id))
				{
					Table_NpcTaskTargetKillDic.Add(_tableNpcTaskTargetKills[i].Id,_tableNpcTaskTargetKills[i]);
				}
				else
				{
					LogHelper.Warning("_tableNpcTaskTargetKills table.Id {0} is duplicated!", _tableNpcTaskTargetKills[i].Id);
				}
			}
			for (int i = 0; i < _tableNpcDefaultDias.Length; i++)
			{
				if (!Table_NpcDefaultDiaDic.ContainsKey(_tableNpcDefaultDias[i].Id))
				{
					Table_NpcDefaultDiaDic.Add(_tableNpcDefaultDias[i].Id,_tableNpcDefaultDias[i]);
				}
				else
				{
					LogHelper.Warning("_tableNpcDefaultDias table.Id {0} is duplicated!", _tableNpcDefaultDias[i].Id);
				}
			}
			for (int i = 0; i < _tableWorkShopNumberOfSlots.Length; i++)
			{
				if (!Table_WorkShopNumberOfSlotDic.ContainsKey(_tableWorkShopNumberOfSlots[i].Id))
				{
					Table_WorkShopNumberOfSlotDic.Add(_tableWorkShopNumberOfSlots[i].Id,_tableWorkShopNumberOfSlots[i]);
				}
				else
				{
					LogHelper.Warning("_tableWorkShopNumberOfSlots table.Id {0} is duplicated!", _tableWorkShopNumberOfSlots[i].Id);
				}
			}
			
			Messenger.Broadcast(EMessengerType.OnTableInited);
		}

		public Table_State GetState(int key)
		{
			Table_State tmp;
			if (Table_StateDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Skill GetSkill(int key)
		{
			Table_Skill tmp;
			if (Table_SkillDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Reward GetReward(int key)
		{
			Table_Reward tmp;
			if (Table_RewardDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_FashionShop GetFashionShop(int key)
		{
			Table_FashionShop tmp;
			if (Table_FashionShopDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Equipment GetEquipment(int key)
		{
			Table_Equipment tmp;
			if (Table_EquipmentDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_EquipmentLevel GetEquipmentLevel(int key)
		{
			Table_EquipmentLevel tmp;
			if (Table_EquipmentLevelDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Trap GetTrap(int key)
		{
			Table_Trap tmp;
			if (Table_TrapDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Unit GetUnit(int key)
		{
			Table_Unit tmp;
			if (Table_UnitDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_CharacterUpgrade GetCharacterUpgrade(int key)
		{
			Table_CharacterUpgrade tmp;
			if (Table_CharacterUpgradeDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_StarRequire GetStarRequire(int key)
		{
			Table_StarRequire tmp;
			if (Table_StarRequireDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_StandaloneChapter GetStandaloneChapter(int key)
		{
			Table_StandaloneChapter tmp;
			if (Table_StandaloneChapterDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_StandaloneLevel GetStandaloneLevel(int key)
		{
			Table_StandaloneLevel tmp;
			if (Table_StandaloneLevelDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Achievement GetAchievement(int key)
		{
			Table_Achievement tmp;
			if (Table_AchievementDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_FashionUnit GetFashionUnit(int key)
		{
			Table_FashionUnit tmp;
			if (Table_FashionUnitDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_HonorReport GetHonorReport(int key)
		{
			Table_HonorReport tmp;
			if (Table_HonorReportDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_TreasureMap GetTreasureMap(int key)
		{
			Table_TreasureMap tmp;
			if (Table_TreasureMapDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Turntable GetTurntable(int key)
		{
			Table_Turntable tmp;
			if (Table_TurntableDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_QQHallGrowAward GetQQHallGrowAward(int key)
		{
			Table_QQHallGrowAward tmp;
			if (Table_QQHallGrowAwardDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_FashionCoupon GetFashionCoupon(int key)
		{
			Table_FashionCoupon tmp;
			if (Table_FashionCouponDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Background GetBackground(int key)
		{
			Table_Background tmp;
			if (Table_BackgroundDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Decorate GetDecorate(int key)
		{
			Table_Decorate tmp;
			if (Table_DecorateDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_DiamondShop GetDiamondShop(int key)
		{
			Table_DiamondShop tmp;
			if (Table_DiamondShopDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_PuzzleSummon GetPuzzleSummon(int key)
		{
			Table_PuzzleSummon tmp;
			if (Table_PuzzleSummonDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Puzzle GetPuzzle(int key)
		{
			Table_Puzzle tmp;
			if (Table_PuzzleDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_PuzzleUpgrade GetPuzzleUpgrade(int key)
		{
			Table_PuzzleUpgrade tmp;
			if (Table_PuzzleUpgradeDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_PuzzleSlot GetPuzzleSlot(int key)
		{
			Table_PuzzleSlot tmp;
			if (Table_PuzzleSlotDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_AvatarStruct GetAvatarStruct(int key)
		{
			Table_AvatarStruct tmp;
			if (Table_AvatarStructDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Matrix GetMatrix(int key)
		{
			Table_Matrix tmp;
			if (Table_MatrixDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_AvatarSlotName GetAvatarSlotName(int key)
		{
			Table_AvatarSlotName tmp;
			if (Table_AvatarSlotNameDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_Morph GetMorph(int key)
		{
			Table_Morph tmp;
			if (Table_MorphDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_PlayerLvToModifyLimit GetPlayerLvToModifyLimit(int key)
		{
			Table_PlayerLvToModifyLimit tmp;
			if (Table_PlayerLvToModifyLimitDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_PlayerLvToExp GetPlayerLvToExp(int key)
		{
			Table_PlayerLvToExp tmp;
			if (Table_PlayerLvToExpDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_ModifyReward GetModifyReward(int key)
		{
			Table_ModifyReward tmp;
			if (Table_ModifyRewardDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_ProgressUnlock GetProgressUnlock(int key)
		{
			Table_ProgressUnlock tmp;
			if (Table_ProgressUnlockDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_BoostItem GetBoostItem(int key)
		{
			Table_BoostItem tmp;
			if (Table_BoostItemDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_NpcTaskTargetColltion GetNpcTaskTargetColltion(int key)
		{
			Table_NpcTaskTargetColltion tmp;
			if (Table_NpcTaskTargetColltionDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_NpcTaskAward GetNpcTaskAward(int key)
		{
			Table_NpcTaskAward tmp;
			if (Table_NpcTaskAwardDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_NpcTaskTargetKill GetNpcTaskTargetKill(int key)
		{
			Table_NpcTaskTargetKill tmp;
			if (Table_NpcTaskTargetKillDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_NpcDefaultDia GetNpcDefaultDia(int key)
		{
			Table_NpcDefaultDia tmp;
			if (Table_NpcDefaultDiaDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}
		public Table_WorkShopNumberOfSlot GetWorkShopNumberOfSlot(int key)
		{
			Table_WorkShopNumberOfSlot tmp;
			if (Table_WorkShopNumberOfSlotDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
		}

		#endregion
	}
}
