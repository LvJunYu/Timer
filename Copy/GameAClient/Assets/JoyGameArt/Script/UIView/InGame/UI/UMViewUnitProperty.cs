using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UMViewUnitProperty : UMViewResManagedBase
    {
        public GameObject ActiveStateDock;
        public GameObject ForwardDirectionDock;
        public GameObject PayloadDock;
        public GameObject MoveDirectionDock;
        public GameObject RotateDock;
        public GameObject DelayDock;
        public GameObject IntervalDock;
        public GameObject ItemDock;
        public GameObject TextDock;

        public Image ActiveStateFg;
        public Image ForwardDirectionFg;
        public Image PayloadFg;
        public Image MoveDirectionFg;
        public Image RotateFgNone;
        public Image RotateFgView;
        public Image ItemSpriteFg;
        public Text DelayText;
        public Text IntervalText;
    }
}