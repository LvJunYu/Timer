using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWorld : UIViewBase
    {
        public Button ReturnBtn;

        public UITabGroup TabGroup;

        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;

        public GameObject RecommendPanel;
        public Button RefreshRecommendButton;
        public RectTransform[] RecommendCardDockAry;

        public GameObject PlayHistoryPanel;
        public GridDataScroller PlayHistoryGridScroller;
        
        public GameObject FavoritePanel;
        public GridDataScroller FavoriteGridScroller;
    }
}
