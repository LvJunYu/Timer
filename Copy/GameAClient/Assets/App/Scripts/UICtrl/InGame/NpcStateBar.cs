using System;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA.Game
{
    public class NpcStateBar : MonoBehaviour
    {
        public const string QuestionMarkSprite = "icon_wenhao";
        public const string ExclamationMarkSprite = "icon_tanhao";
        public const string GrayQuestionMarkSprite = "icon_wenhao_2";

        public TextMesh SerNumMesh;
        private Transform _trans;
        public SpriteRenderer TaskSignSprite;
        public TextMesh ConTip;
        public TextMesh NameTextMesh;


        void Awake()
        {
            _trans = transform;
            TaskSignSprite.sortingOrder = (int) ESortingOrder.DragingItem;
        }

        void Start()
        {
        }

        void Update()
        {
            _trans.rotation = Quaternion.identity;
        }

        public void SetNpcNum(int num)
        {
            SerNumMesh.SetActiveEx(true);
            TaskSignSprite.SetActiveEx(false);
            ConTip.SetActiveEx(false);
            SerNumMesh.text = num.ToString();
        }

        public void SetReady()
        {
            Sprite _exclamationSprite;
            SerNumMesh.SetActiveEx(false);
            TaskSignSprite.SetActiveEx(true);
            ConTip.SetActiveEx(false);
            JoyResManager.Instance.TryGetSprite(ExclamationMarkSprite, out _exclamationSprite);
            TaskSignSprite.sprite = _exclamationSprite;
        }

        public void SetInTask()
        {
            Sprite _grayQuestionSprite;
            SerNumMesh.SetActiveEx(false);
            TaskSignSprite.SetActiveEx(true);
            ConTip.SetActiveEx(false);
            JoyResManager.Instance.TryGetSprite(GrayQuestionMarkSprite, out _grayQuestionSprite);
            TaskSignSprite.sprite = _grayQuestionSprite;
        }

        public void AllTaskFinish()
        {
            Sprite _questionSprite;
            SerNumMesh.SetActiveEx(false);
            TaskSignSprite.SetActiveEx(true);
            ConTip.SetActiveEx(false);
            JoyResManager.Instance.TryGetSprite(QuestionMarkSprite, out _questionSprite);
            TaskSignSprite.sprite = _questionSprite;
        }

        public void ShowTip()
        {
            SerNumMesh.SetActiveEx(false);
            TaskSignSprite.SetActiveEx(false);
            ConTip.SetActiveEx(true);
            ConTip.text = String.Format("按{0}键对话",
                CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagAssist));
        }

        public void SetNoShow()
        {
            SerNumMesh.SetActiveEx(false);
            TaskSignSprite.SetActiveEx(false);
            ConTip.SetActiveEx(false);
        }
    }
}