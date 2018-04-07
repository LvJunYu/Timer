using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UMViewGMProjectOfficalRecommend : UMViewResManagedBase
    {
        public Text TileText;
        public RawImage ProjectBgImage;
//        public GameObject SingleObj;
//        public GameObject CooperationObj;
//        public GameObject CompeteObj;
        public Button SelectBtn;
        public Button UnSelectBtn;
        public Texture DefualtTexture;
        public GameObject ProjectObj;
        public ProjectDragHelper ProjectDragHelper;
        public RectTransform ProjectRect;
    }
}