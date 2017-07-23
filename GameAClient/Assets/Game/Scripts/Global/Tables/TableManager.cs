using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
namespace GameA.Game
{
	[Serializable]
	public class TableManager : MonoBehaviour
	{
		#region 常量与字段
		// #if UNITY_EDITOR
		private const string editorJsonDataPath = "JsonTableData/";
		// #endif
		private static TableManager _instance;
		public readonly Dictionary<int,Table_Equipment> Table_EquipmentDic = new Dictionary<int, Table_Equipment>();
		public readonly Dictionary<int,Table_Skill> Table_SkillDic = new Dictionary<int, Table_Skill>();
		public readonly Dictionary<int,Table_State> Table_StateDic = new Dictionary<int, Table_State>();
		public readonly Dictionary<int,Table_Trap> Table_TrapDic = new Dictionary<int, Table_Trap>();
		public readonly Dictionary<int,Table_Unit> Table_UnitDic = new Dictionary<int, Table_Unit>();
		public readonly Dictionary<int,Table_StandaloneLevel> Table_StandaloneLevelDic = new Dictionary<int, Table_StandaloneLevel>();
		public readonly Dictionary<int,Table_StarRequire> Table_StarRequireDic = new Dictionary<int, Table_StarRequire>();
		public readonly Dictionary<int,Table_StandaloneChapter> Table_StandaloneChapterDic = new Dictionary<int, Table_StandaloneChapter>();
		public readonly Dictionary<int,Table_Reward> Table_RewardDic = new Dictionary<int, Table_Reward>();
		public readonly Dictionary<int,Table_FashionShop> Table_FashionShopDic = new Dictionary<int, Table_FashionShop>();
		public readonly Dictionary<int,Table_FashionUnit> Table_FashionUnitDic = new Dictionary<int, Table_FashionUnit>();
		public readonly Dictionary<int,Table_TreasureMap> Table_TreasureMapDic = new Dictionary<int, Table_TreasureMap>();
		public readonly Dictionary<int,Table_Turntable> Table_TurntableDic = new Dictionary<int, Table_Turntable>();
		public readonly Dictionary<int,Table_FashionCoupon> Table_FashionCouponDic = new Dictionary<int, Table_FashionCoupon>();
		public readonly Dictionary<int,Table_Background> Table_BackgroundDic = new Dictionary<int, Table_Background>();
		public readonly Dictionary<int,Table_Decorate> Table_DecorateDic = new Dictionary<int, Table_Decorate>();
		public readonly Dictionary<int,Table_Matrix> Table_MatrixDic = new Dictionary<int, Table_Matrix>();
		public readonly Dictionary<int,Table_Morph> Table_MorphDic = new Dictionary<int, Table_Morph>();
		public readonly Dictionary<int,Table_PuzzleSummon> Table_PuzzleSummonDic = new Dictionary<int, Table_PuzzleSummon>();
		public readonly Dictionary<int,Table_Puzzle> Table_PuzzleDic = new Dictionary<int, Table_Puzzle>();
		public readonly Dictionary<int,Table_AvatarStruct> Table_AvatarStructDic = new Dictionary<int, Table_AvatarStruct>();
		public readonly Dictionary<int,Table_AvatarSlotName> Table_AvatarSlotNameDic = new Dictionary<int, Table_AvatarSlotName>();
		public readonly Dictionary<int,Table_PlayerLvToModifyLimit> Table_PlayerLvToModifyLimitDic = new Dictionary<int, Table_PlayerLvToModifyLimit>();
		public readonly Dictionary<int,Table_PlayerLvToExp> Table_PlayerLvToExpDic = new Dictionary<int, Table_PlayerLvToExp>();
		public readonly Dictionary<int,Table_ModifyReward> Table_ModifyRewardDic = new Dictionary<int, Table_ModifyReward>();
		public readonly Dictionary<int,Table_ProgressUnlock> Table_ProgressUnlockDic = new Dictionary<int, Table_ProgressUnlock>();
		public readonly Dictionary<int,Table_BoostItem> Table_BoostItemDic = new Dictionary<int, Table_BoostItem>();
		[UnityEngine.SerializeField] private Table_Equipment[] _tableEquipments;
		[UnityEngine.SerializeField] private Table_Skill[] _tableSkills;
		[UnityEngine.SerializeField] private Table_State[] _tableStates;
		[UnityEngine.SerializeField] private Table_Trap[] _tableTraps;
		[UnityEngine.SerializeField] private Table_Unit[] _tableUnits;
		[UnityEngine.SerializeField] private Table_StandaloneLevel[] _tableStandaloneLevels;
		[UnityEngine.SerializeField] private Table_StarRequire[] _tableStarRequires;
		[UnityEngine.SerializeField] private Table_StandaloneChapter[] _tableStandaloneChapters;
		[UnityEngine.SerializeField] private Table_Reward[] _tableRewards;
		[UnityEngine.SerializeField] private Table_FashionShop[] _tableFashionShops;
		[UnityEngine.SerializeField] private Table_FashionUnit[] _tableFashionUnits;
		[UnityEngine.SerializeField] private Table_TreasureMap[] _tableTreasureMaps;
		[UnityEngine.SerializeField] private Table_Turntable[] _tableTurntables;
		[UnityEngine.SerializeField] private Table_FashionCoupon[] _tableFashionCoupons;
		[UnityEngine.SerializeField] private Table_Background[] _tableBackgrounds;
		[UnityEngine.SerializeField] private Table_Decorate[] _tableDecorates;
		[UnityEngine.SerializeField] private Table_Matrix[] _tableMatrixs;
		[UnityEngine.SerializeField] private Table_Morph[] _tableMorphs;
		[UnityEngine.SerializeField] private Table_PuzzleSummon[] _tablePuzzleSummons;
		[UnityEngine.SerializeField] private Table_Puzzle[] _tablePuzzles;
		[UnityEngine.SerializeField] private Table_AvatarStruct[] _tableAvatarStructs;
		[UnityEngine.SerializeField] private Table_AvatarSlotName[] _tableAvatarSlotNames;
		[UnityEngine.SerializeField] private Table_PlayerLvToModifyLimit[] _tablePlayerLvToModifyLimits;
		[UnityEngine.SerializeField] private Table_PlayerLvToExp[] _tablePlayerLvToExps;
		[UnityEngine.SerializeField] private Table_ModifyReward[] _tableModifyRewards;
		[UnityEngine.SerializeField] private Table_ProgressUnlock[] _tableProgressUnlocks;
		[UnityEngine.SerializeField] private Table_BoostItem[] _tableBoostItems;

		//private TableResLoader _loader;
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
			// #if UNITY_EDITOR
			var EquipmentTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Equipment"));
			_tableEquipments = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Equipment[]>(EquipmentTextAsset.text);
			var SkillTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Skill"));
			_tableSkills = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Skill[]>(SkillTextAsset.text);
			var StateTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "State"));
			_tableStates = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_State[]>(StateTextAsset.text);
			var TrapTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Trap"));
			_tableTraps = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Trap[]>(TrapTextAsset.text);
			var UnitTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Unit"));
			_tableUnits = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Unit[]>(UnitTextAsset.text);
			var StandaloneLevelTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "StandaloneLevel"));
			_tableStandaloneLevels = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_StandaloneLevel[]>(StandaloneLevelTextAsset.text);
			var StarRequireTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "StarRequire"));
			_tableStarRequires = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_StarRequire[]>(StarRequireTextAsset.text);
			var StandaloneChapterTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "StandaloneChapter"));
			_tableStandaloneChapters = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_StandaloneChapter[]>(StandaloneChapterTextAsset.text);
			var RewardTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Reward"));
			_tableRewards = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Reward[]>(RewardTextAsset.text);
			var FashionShopTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "FashionShop"));
			_tableFashionShops = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_FashionShop[]>(FashionShopTextAsset.text);
			var FashionUnitTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "FashionUnit"));
			_tableFashionUnits = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_FashionUnit[]>(FashionUnitTextAsset.text);
			var TreasureMapTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "TreasureMap"));
			_tableTreasureMaps = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_TreasureMap[]>(TreasureMapTextAsset.text);
			var TurntableTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Turntable"));
			_tableTurntables = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Turntable[]>(TurntableTextAsset.text);
			var FashionCouponTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "FashionCoupon"));
			_tableFashionCoupons = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_FashionCoupon[]>(FashionCouponTextAsset.text);
			var BackgroundTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Background"));
			_tableBackgrounds = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Background[]>(BackgroundTextAsset.text);
			var DecorateTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Decorate"));
			_tableDecorates = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Decorate[]>(DecorateTextAsset.text);
			var MatrixTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Matrix"));
			_tableMatrixs = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Matrix[]>(MatrixTextAsset.text);
			var MorphTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Morph"));
			_tableMorphs = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Morph[]>(MorphTextAsset.text);
			var PuzzleSummonTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "PuzzleSummon"));
			_tablePuzzleSummons = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_PuzzleSummon[]>(PuzzleSummonTextAsset.text);
			var PuzzleTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "Puzzle"));
			_tablePuzzles = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_Puzzle[]>(PuzzleTextAsset.text);
			var AvatarStructTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "AvatarStruct"));
			_tableAvatarStructs = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_AvatarStruct[]>(AvatarStructTextAsset.text);
			var AvatarSlotNameTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "AvatarSlotName"));
			_tableAvatarSlotNames = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_AvatarSlotName[]>(AvatarSlotNameTextAsset.text);
			var PlayerLvToModifyLimitTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "PlayerLvToModifyLimit"));
			_tablePlayerLvToModifyLimits = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_PlayerLvToModifyLimit[]>(PlayerLvToModifyLimitTextAsset.text);
			var PlayerLvToExpTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "PlayerLvToExp"));
			_tablePlayerLvToExps = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_PlayerLvToExp[]>(PlayerLvToExpTextAsset.text);
			var ModifyRewardTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "ModifyReward"));
			_tableModifyRewards = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_ModifyReward[]>(ModifyRewardTextAsset.text);
			var ProgressUnlockTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "ProgressUnlock"));
			_tableProgressUnlocks = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_ProgressUnlock[]>(ProgressUnlockTextAsset.text);
			var BoostItemTextAsset = Resources.Load<TextAsset>(string.Format("{0}{1}", editorJsonDataPath, "BoostItem"));
			_tableBoostItems = Newtonsoft.Json.JsonConvert.DeserializeObject<Table_BoostItem[]>(BoostItemTextAsset.text);
			// #else
			// _loader = new TableResLoader("GameMaker2D");

            // 
			// var EquipmentAsset = _loader.GetConfigAssetData<TableEquipmentAsset>("Equipment");
			// if (EquipmentAsset == null)
			// {
			// 	LogHelper.Error("EquipmentAsset is null");
			// 	return;
			// }
			// _tableEquipments = EquipmentAsset.DataArray;
			// 
			// var SkillAsset = _loader.GetConfigAssetData<TableSkillAsset>("Skill");
			// if (SkillAsset == null)
			// {
			// 	LogHelper.Error("SkillAsset is null");
			// 	return;
			// }
			// _tableSkills = SkillAsset.DataArray;
			// 
			// var StateAsset = _loader.GetConfigAssetData<TableStateAsset>("State");
			// if (StateAsset == null)
			// {
			// 	LogHelper.Error("StateAsset is null");
			// 	return;
			// }
			// _tableStates = StateAsset.DataArray;
			// 
			// var TrapAsset = _loader.GetConfigAssetData<TableTrapAsset>("Trap");
			// if (TrapAsset == null)
			// {
			// 	LogHelper.Error("TrapAsset is null");
			// 	return;
			// }
			// _tableTraps = TrapAsset.DataArray;
			// 
			// var UnitAsset = _loader.GetConfigAssetData<TableUnitAsset>("Unit");
			// if (UnitAsset == null)
			// {
			// 	LogHelper.Error("UnitAsset is null");
			// 	return;
			// }
			// _tableUnits = UnitAsset.DataArray;
			// 
			// var StandaloneLevelAsset = _loader.GetConfigAssetData<TableStandaloneLevelAsset>("StandaloneLevel");
			// if (StandaloneLevelAsset == null)
			// {
			// 	LogHelper.Error("StandaloneLevelAsset is null");
			// 	return;
			// }
			// _tableStandaloneLevels = StandaloneLevelAsset.DataArray;
			// 
			// var StarRequireAsset = _loader.GetConfigAssetData<TableStarRequireAsset>("StarRequire");
			// if (StarRequireAsset == null)
			// {
			// 	LogHelper.Error("StarRequireAsset is null");
			// 	return;
			// }
			// _tableStarRequires = StarRequireAsset.DataArray;
			// 
			// var StandaloneChapterAsset = _loader.GetConfigAssetData<TableStandaloneChapterAsset>("StandaloneChapter");
			// if (StandaloneChapterAsset == null)
			// {
			// 	LogHelper.Error("StandaloneChapterAsset is null");
			// 	return;
			// }
			// _tableStandaloneChapters = StandaloneChapterAsset.DataArray;
			// 
			// var RewardAsset = _loader.GetConfigAssetData<TableRewardAsset>("Reward");
			// if (RewardAsset == null)
			// {
			// 	LogHelper.Error("RewardAsset is null");
			// 	return;
			// }
			// _tableRewards = RewardAsset.DataArray;
			// 
			// var FashionShopAsset = _loader.GetConfigAssetData<TableFashionShopAsset>("FashionShop");
			// if (FashionShopAsset == null)
			// {
			// 	LogHelper.Error("FashionShopAsset is null");
			// 	return;
			// }
			// _tableFashionShops = FashionShopAsset.DataArray;
			// 
			// var FashionUnitAsset = _loader.GetConfigAssetData<TableFashionUnitAsset>("FashionUnit");
			// if (FashionUnitAsset == null)
			// {
			// 	LogHelper.Error("FashionUnitAsset is null");
			// 	return;
			// }
			// _tableFashionUnits = FashionUnitAsset.DataArray;
			// 
			// var TreasureMapAsset = _loader.GetConfigAssetData<TableTreasureMapAsset>("TreasureMap");
			// if (TreasureMapAsset == null)
			// {
			// 	LogHelper.Error("TreasureMapAsset is null");
			// 	return;
			// }
			// _tableTreasureMaps = TreasureMapAsset.DataArray;
			// 
			// var TurntableAsset = _loader.GetConfigAssetData<TableTurntableAsset>("Turntable");
			// if (TurntableAsset == null)
			// {
			// 	LogHelper.Error("TurntableAsset is null");
			// 	return;
			// }
			// _tableTurntables = TurntableAsset.DataArray;
			// 
			// var FashionCouponAsset = _loader.GetConfigAssetData<TableFashionCouponAsset>("FashionCoupon");
			// if (FashionCouponAsset == null)
			// {
			// 	LogHelper.Error("FashionCouponAsset is null");
			// 	return;
			// }
			// _tableFashionCoupons = FashionCouponAsset.DataArray;
			// 
			// var BackgroundAsset = _loader.GetConfigAssetData<TableBackgroundAsset>("Background");
			// if (BackgroundAsset == null)
			// {
			// 	LogHelper.Error("BackgroundAsset is null");
			// 	return;
			// }
			// _tableBackgrounds = BackgroundAsset.DataArray;
			// 
			// var DecorateAsset = _loader.GetConfigAssetData<TableDecorateAsset>("Decorate");
			// if (DecorateAsset == null)
			// {
			// 	LogHelper.Error("DecorateAsset is null");
			// 	return;
			// }
			// _tableDecorates = DecorateAsset.DataArray;
			// 
			// var MatrixAsset = _loader.GetConfigAssetData<TableMatrixAsset>("Matrix");
			// if (MatrixAsset == null)
			// {
			// 	LogHelper.Error("MatrixAsset is null");
			// 	return;
			// }
			// _tableMatrixs = MatrixAsset.DataArray;
			// 
			// var MorphAsset = _loader.GetConfigAssetData<TableMorphAsset>("Morph");
			// if (MorphAsset == null)
			// {
			// 	LogHelper.Error("MorphAsset is null");
			// 	return;
			// }
			// _tableMorphs = MorphAsset.DataArray;
			// 
			// var PuzzleSummonAsset = _loader.GetConfigAssetData<TablePuzzleSummonAsset>("PuzzleSummon");
			// if (PuzzleSummonAsset == null)
			// {
			// 	LogHelper.Error("PuzzleSummonAsset is null");
			// 	return;
			// }
			// _tablePuzzleSummons = PuzzleSummonAsset.DataArray;
			// 
			// var PuzzleAsset = _loader.GetConfigAssetData<TablePuzzleAsset>("Puzzle");
			// if (PuzzleAsset == null)
			// {
			// 	LogHelper.Error("PuzzleAsset is null");
			// 	return;
			// }
			// _tablePuzzles = PuzzleAsset.DataArray;
			// 
			// var AvatarStructAsset = _loader.GetConfigAssetData<TableAvatarStructAsset>("AvatarStruct");
			// if (AvatarStructAsset == null)
			// {
			// 	LogHelper.Error("AvatarStructAsset is null");
			// 	return;
			// }
			// _tableAvatarStructs = AvatarStructAsset.DataArray;
			// 
			// var AvatarSlotNameAsset = _loader.GetConfigAssetData<TableAvatarSlotNameAsset>("AvatarSlotName");
			// if (AvatarSlotNameAsset == null)
			// {
			// 	LogHelper.Error("AvatarSlotNameAsset is null");
			// 	return;
			// }
			// _tableAvatarSlotNames = AvatarSlotNameAsset.DataArray;
			// 
			// var PlayerLvToModifyLimitAsset = _loader.GetConfigAssetData<TablePlayerLvToModifyLimitAsset>("PlayerLvToModifyLimit");
			// if (PlayerLvToModifyLimitAsset == null)
			// {
			// 	LogHelper.Error("PlayerLvToModifyLimitAsset is null");
			// 	return;
			// }
			// _tablePlayerLvToModifyLimits = PlayerLvToModifyLimitAsset.DataArray;
			// 
			// var PlayerLvToExpAsset = _loader.GetConfigAssetData<TablePlayerLvToExpAsset>("PlayerLvToExp");
			// if (PlayerLvToExpAsset == null)
			// {
			// 	LogHelper.Error("PlayerLvToExpAsset is null");
			// 	return;
			// }
			// _tablePlayerLvToExps = PlayerLvToExpAsset.DataArray;
			// 
			// var ModifyRewardAsset = _loader.GetConfigAssetData<TableModifyRewardAsset>("ModifyReward");
			// if (ModifyRewardAsset == null)
			// {
			// 	LogHelper.Error("ModifyRewardAsset is null");
			// 	return;
			// }
			// _tableModifyRewards = ModifyRewardAsset.DataArray;
			// 
			// var ProgressUnlockAsset = _loader.GetConfigAssetData<TableProgressUnlockAsset>("ProgressUnlock");
			// if (ProgressUnlockAsset == null)
			// {
			// 	LogHelper.Error("ProgressUnlockAsset is null");
			// 	return;
			// }
			// _tableProgressUnlocks = ProgressUnlockAsset.DataArray;
			// 
			// var BoostItemAsset = _loader.GetConfigAssetData<TableBoostItemAsset>("BoostItem");
			// if (BoostItemAsset == null)
			// {
			// 	LogHelper.Error("BoostItemAsset is null");
			// 	return;
			// }
			// _tableBoostItems = BoostItemAsset.DataArray;
			// 
			// #endif
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
			
			Messenger.Broadcast(EMessengerType.OnTableInited);
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
		public Table_Skill GetSkill(int key)
		{
			Table_Skill tmp;
			if (Table_SkillDic.TryGetValue(key,out tmp))
			{
				return tmp;
			}
			return null;
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
		public Table_StandaloneLevel GetStandaloneLevel(int key)
		{
			Table_StandaloneLevel tmp;
			if (Table_StandaloneLevelDic.TryGetValue(key,out tmp))
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
		public Table_FashionUnit GetFashionUnit(int key)
		{
			Table_FashionUnit tmp;
			if (Table_FashionUnitDic.TryGetValue(key,out tmp))
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
		public Table_Matrix GetMatrix(int key)
		{
			Table_Matrix tmp;
			if (Table_MatrixDic.TryGetValue(key,out tmp))
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
		public Table_AvatarStruct GetAvatarStruct(int key)
		{
			Table_AvatarStruct tmp;
			if (Table_AvatarStructDic.TryGetValue(key,out tmp))
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

		#endregion
	}
}
