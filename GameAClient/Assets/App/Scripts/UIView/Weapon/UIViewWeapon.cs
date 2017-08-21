using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWeapon : UIViewBase
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
        public Image OwnedUniversalFragmentsIcon;
        public Image OwnedWeaponFragmentsIcon;
        public Image LockedImage;
        public Transform EffectShow;
        public Button LeftWeapon;
        public Button RightWeapon;
        public Button CloseButton;
        public Button UpGrade;
    }
}
