using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewUnitPropertyEdit : UIViewResManagedBase
    {
        public RectTransform ContentDock;
        public Button CloseBtn;
        public GameObject ActiveDock;
        public GameObject ForwardDock;
        public GameObject PayloadDock;
        public GameObject MoveDirectionDock;
        public GameObject AngelDock;
        public GameObject RotateDock;
        public GameObject RotateModeDock;
        public GameObject RotateEndDock;
        public GameObject TriggerDelayDock;
        public GameObject TriggerIntervalDock;
        public GameObject TextDock;
        public GameObject RotateArrowDock;
        public GameObject CampDock;
        public GameObject MonsterCaveDock;
        public GameObject SpawnDock;
        public GameObject SurpriseBoxDock;
        public GameObject WoodCaseDock;
        public GameObject PasswordDoorDock;
        public GameObject BombDock;
        public GameObject CycleDock;
        public GameObject NpcTypeDock;
        public GameObject NpcDiaLogDock;
        public GameObject NpcTaskDock;

        public Image RotateViewImage;
        public Image AngelViewImage;
        public InputField TextInput;

        public USViewUnitPropertyEditButton ActiveStateMenu;
        public USViewUnitPropertyEditButton DirectionMenu;
        public USViewUnitPropertyEditButton PayloadMenu;
        public USViewUnitPropertyEditButton MoveDirectionMenu;
        public USViewUnitPropertyEditButton RotateStateMenu;
        public USViewUnitPropertyEditButton AngelMenu;
        public USViewUnitPropertyEditButton TimeDelayMenu;
        public USViewUnitPropertyEditButton TimeIntervalMenu;
        public USViewUnitPropertyEditButton TextMenu;
        public USViewUnitPropertyEditButton CampMenu;
        public USViewUnitPropertyEditButton MonsterCaveMenu;
        public USViewUnitPropertyEditButton SpawnMenu;
        public USViewUnitPropertyEditButton SurpriseBoxMenu;
        public USViewUnitPropertyEditButton WoodCaseMenu;
        public USViewUnitPropertyEditButton PasswordDoorMenu;
        public USViewUnitPropertyEditButton BombMenu;
        public USViewUnitPropertyEditButton CycleMenu;
        public USViewUnitPropertyEditButton NpcTypeMenu;
        public USViewUnitPropertyEditButton NpcTaskSettingMenu;

        public ToggleGroup PreinstallToggleGroup;
        public Button SavePreinstallBtn;
        public Button DeletePreinstallBtn;
        public Button CreatePreinstallBtn;
        public GameObject PreinstallBtns;

        public USViewSliderSetting SurpriseBoxIntervalSetting;
        public USViewSliderSetting SurpriseBoxMaxCountSetting;
        public USViewGameSettingItem SurpriseBoxRandomSetting;
        public USViewGameSettingItem SurpriseBoxLimitSetting;

        public GameObject AdvancePannel;
        public RectTransform AdvanceContentRtf;
        public USViewSliderSetting MaxHpSetting;
        public USViewSliderSetting JumpSetting;
        public USViewSliderSetting MoveSpeedSetting;
        public USViewSliderSetting DamageSetting;
        public USViewSliderSetting EffectRangeSetting;
        public USViewSliderSetting CastRangeSetting;
        public USViewSliderSetting DamageIntervalSetting;
        public USViewSliderSetting BulletSpeedSetting;
        public USViewSliderSetting BulletCountSetting;
        public USViewSliderSetting ChargeTimeSetting;
        public USViewSliderSetting InjuredReduceSetting;
        public USViewSliderSetting CurIncreaseSetting;
        public USViewSliderSetting MonsterIntervalTimeSetting;
        public USViewSliderSetting MaxCreatedMonsterSetting;
        public USViewSliderSetting MaxAliveMonsterSetting;
        public USViewSliderSetting TimerSecondSetting;
        public USViewSliderSetting TimerMinSecondSetting;
        public USViewSliderSetting TimerMaxSecondSetting;
        public USViewGameSettingItem CanMoveSetting;
        public USViewGameSettingItem TimerRandomSetting;
        public USViewGameSettingItem TimerCirculationSetting;
        public USViewAddItem DropsSetting;
        public USViewAddItem AddStatesSetting;
        public USViewSetItem PlayerWeaponSetting;
        public USViewDropdownSetting SpawnSetting;
        public USViewSurpriseBoxItemsSetting SurpriseBoxItemSetting;
        public USViewWoodCaseItemsSetting WoodCaseItemsSetting;
        public USViewPasswordDoorSetting PasswordDoorSetting;

        public Button MonsterSettingBtn;
        public Button BackBtn;
        public RawImage CharacterRawImage;

        //Npc对话设置
        [Header("Npc对话设置")] public InputField NpcName;

        public InputField NpcDialog;
        public USViewSliderSetting DialogShowIntervalTimeSetting;
        public USViewUnitPropertyEditButton NpcCloseToShowBtn;
        public USViewUnitPropertyEditButton NpcIntervalShowBtn;
        public Text NameTextNum;
        public Text DiaTextNum;


        //Npc任务设置
        [Header("Npc任务设置")] public USViewUnitNpcTaskIndexBtn[] IndexBtnGroup;

        public USViewUnitNpcTaskTarget[] TargetBtnGroup;
        public Text TaskSerialNumText;
        public Button DialogBtn;
        public Button AddConditionBtn;
        public InputField TaskNpcName;
        public Text NameNumText;
        public Button AddTargetBtn;
        public USViewSliderSetting TaskTimeLimitSetting;
        public GameObject TaskTimeLimitObj;
        public Button RemoveTaskTimeLimitBtn;
        public USViewUnitNpcTaskTarget TriggerConditonBtn;
        public GameObject TriggerConditonObj;
        public GameObject NoTargetTip;


        [Header("选择任务目标种类")] public GameObject NpcTaskTypePanel;
        public Button[] TargetTypeBtnGroup;
        public Button NpcTaskTypeExitBtn;

        //打怪任务
        [Header("打怪任务")] public GameObject NpcTaskMonsterPanel;

        public USViewSliderSetting KillNumSetting;
        public RectTransform ItemContent;
        public Button MonsterUpBtn;
        public Button MonsterDownBtn;
        public Button NpcTaskMonsterPanelExitBtn;

        public Scrollbar MonsterBar;


        //收集
        [Header("收集")] public GameObject
            NpcTaskColltionPanel;

        public USViewSliderSetting ColltionNumSetting;
        public RectTransform ColltionItemContent;
        public Button ColltionUpBtn;
        public Button ColltionDownBtn;
        public Scrollbar ColltionMBar;
        public Button NpcTaskColltionPanelExitBtn;


        //任务为传话
        [Header("任务为传话")] public GameObject NpcTaskDiaPanel;

        public InputField DiaTargetNpcNum;
        public Button NpcTaskDiaPanelExitBtn;
        public Text NoNpcTipText;

        //增加npc的条件
        [Header("增加npc的条件")] public GameObject NpcTaskAddConditionPanel;

        public RectTransform NpcTaskAddConditionContentRtf;
        public Button TriggerCondtionBtn;

        public Button TaskTimeLimit;
        public Button NpcTaskAddConditionPanelExitBtn;

        //增加触发条件的类型选择
        [Header("触发条件")] public GameObject NpcTaskConditionTypePanel;

        public Button[] CondtionTypeBtnGroup;
        public Button NpcTaskConditionTypePanelExitBtn;

        //前置任务的面板
        [Header("前置任务的面板")] public GameObject NpcTaskBeforeConditionTypePanel;

        public InputField BeforeConditionInputField;
        public Button NpcTaskBeforeConditionTypePanelExitBtn;

        //编辑对话面板
        [Header("编辑对话面板")] public GameObject NpcTaskEditDiaPanel;

        public Button TaskDiaBeforeBtn;
        public Button TaskMidBeforeBtn;
        public Button TaskAfterBeforeBtn;
        public InputField TargetTaskNpc;
        public USViewUnitNpcTaskTarget[] BeforeTaskAward;
        public USViewUnitNpcTaskTarget[] FinishTaskAward;
        public Button AddBeforeTaskAwardBtn;
        public Button AddFinishTaskAwardBtn;
        public Button NpcTaskEditDiaPanelExitBtn;

        [Header("选择任务前奖励目标种类")] public GameObject NpcTaskBeforeAwardTypePanel;
        public Button[] BeforeAwardTypeTypeBtnGroup;
        public Button NpcTaskBeforeAwardTypePanelExitBtn;

        [Header("选择任务后奖励目标种类")] public GameObject NpcTaskFinishAwardTypePanel;
        public Button[] FinishAwardTypeTypeBtnGroup;
        public Button NpcTaskFinishAwardTypePanelExitBtn;

        [Header("任务奖励")] public GameObject NpcTaskAwardColltionPanel;
        public USViewSliderSetting TaskAwardColltionNumSetting;
        public RectTransform TaskAwardColltionItemContent;
        public Button TaskAwardColltionUpBtn;
        public Button TaskAwardColltionDownBtn;
        public Scrollbar TaskAwardColltionMBar;
        public Button TaskAwardNpcTaskColltionPanelExitBtn;
    }
}