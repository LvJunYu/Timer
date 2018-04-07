using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewEditNpcDia : UIViewResManagedBase
    {
        public Button ExitBtn;
        public Button ExitMaskBtn;

        //选择是npc或者是主角的按钮
        public UITabGroup NpcTypeTabGroup;

        public Button[] TypeButtonAry;

        public Button[] TypeSelectedButtonAry;

        //选择表情的按钮
        public UITabGroup IconGroup;

        public Button[] IconButtonAry;

        public Button[] IconSelectedButtonAry;

        //选择晃动方式
        public Button SlideDownBtn;

        public Text NowWaggleText;

        public Transform WaggleBtnContenTrans;
        public UITabGroup WaggleGroup;
        public Button[] WaggleButtonAry;

        public Button[] WaggleSelectedButtonAry;

        //对话输入框
        public SpecialInputField DiaInputField;

        public Text StrLengthText;
        public Text ShowDiaText;

        //颜色
        public UITabGroup ColorGroup;

        public Button[] ColorButtonAry;

        public Button[] ColorSelectedButtonAry;

        //确认按钮
        public Button ConfirmBtn;

        public Button ConfirmBtnMask;

        public USViewNpcDiaItem[] NpcDiaItem;

        //常用
        public Button CommonUseBtn;

        public RectTransform CommonContentParentTrs;
        public RectTransform ConmmonContentTrs;
        public Button CommonDiaUpBtn;
        public Button CommonDiaDownBtn;
        public Scrollbar Bar;

        //对话列表
        public Button UpBtn;
        public Button DownBtn;
        public Scrollbar DiaItemBar;
        public RectTransform DiaItemContent;
        public RectTransform DiaItemContentView;
        public ScrollRect DiaScorllRect;
    }
}