using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewMailDetail : UIViewResManagedBase
    {
        public Text UITitle;
        public Text Title;
        public Text Content;
        public Text TitleAuthor;
        public Text ContentAuthor;
        public Button CloseBtn;
        public Button OKBtn;
        public Button DeleteBtn;
        public Text OKBtnTxt;
        public USViewGameFinishReward[] Rewards;
        public GameObject RewardPannel;
        public GameObject ProjectPannel;
        public GameObject ShadowBattlePannel;
        public GameObject NormalPannel;
        public RawImage ShadowBattleRawImage;
        public RawImage ProjectRawImage;
        public Button ShadowBattleBtn;
        public Button ProjectBtn;
        public Texture DefaltTexture;
    }
}