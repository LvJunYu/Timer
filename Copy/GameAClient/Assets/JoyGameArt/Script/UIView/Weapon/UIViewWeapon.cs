using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWeapon : UIViewResManagedBase
    {
        public Text UnlockedWeaponNum;
        public Text OwnedUniversalFragmentsNum;
        public Text OwnedWeaponFragmentsNum;
        public Text WeaponName;
        public Text HpAddNum;
        public Text AttackAddNum;
        public Text SkillDescription;
        public Text CostGolCoinNum;
        public Text CostWeaponFragmentsNum;
        public Text WeaponLv;
        public Text UpGradeOrCompound;
        public Image CostPartIcons;
        public Image OwnedWeaponFragmentsIcon;
        public Image LockedImage;
        public RawImage EffectImage;
        public Transform EffectShow;
        public Button LeftWeapon;
        public Button RightWeapon;
        public Button CloseButton;
        public Button UpGrade;
    }
}