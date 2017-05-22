/********************************************************************
** Filename : UICommentInput.cs
** Author : quan
** Date : 2016/8/3 15:25
** Summary : UICommentInput.cs
***********************************************************************/
using System;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UICommentInput : MonoBehaviour
    {
        public GameObject ReplyDock;
        public Button ReplyCancelBtn;
        public Text ReplyNameLabel;
        public InputField Input;
		/// <summary>
		/// 发送按钮
		/// </summary>
        public Button SendBtn;

        private User _targetUser;

        public User TargetUser
        {
            get { return _targetUser; }
        }

        private void Awake()
        {
            ReplyCancelBtn.onClick.AddListener(OnReplyCancelBtnClick);
        }

        public void ClearAll()
        {
            ClearInput();
            ClearTargetUser();
        }

        public void ClearInput()
        {
            Input.text = string.Empty;
        }

        public void ClearTargetUser()
        {
            _targetUser = null;
            ReplyDock.SetActive(false);
        }

        public void SetTargetUser(User user)
        {
            _targetUser = user;
            ReplyDock.SetActive(true);
            DictionaryTools.SetContentText(ReplyNameLabel, _targetUser.NickName);
        }

        private void OnReplyCancelBtnClick()
        {
            ClearTargetUser();
        }
    }
}

