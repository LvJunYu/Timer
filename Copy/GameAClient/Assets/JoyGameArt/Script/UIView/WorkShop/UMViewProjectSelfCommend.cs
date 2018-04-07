using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UMViewProjectSelfCommend : UMViewResManagedBase
    {
        public Button AddProjectBtn;
        public Text IndexText;
        public Image LockImage;
        public Text TileText;
        public RawImage ProjectBgImage;
        public Button SelectBtn;
        public Button UnSelectBtn;
        public Texture DefualtTexture;
        public GameObject ProjectObj;
        public RawImage BgImage;
        public Text LastTimeText;
        public ProjectDragHelper ProjectDragHelper;
        public RectTransform ProjectRect;
    }
}