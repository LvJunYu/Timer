/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using SoyEngine;
using GameA.Game;

namespace GameA
{
    public class USCtrlChallengeProjectCard : USCtrlBase<USViewChallengeProjectCard>
    {
        #region 常量与字段
        private static string _unkown = "?";
        private EChallengeProjectType _type;
        private Project _project;
        #endregion

        #region 属性


        #endregion

        #region 方法
        public override void Init (USViewChallengeProjectCard view)
        {
            base.Init (view);
        }

        protected override void OnViewCreated ()
        {
            base.OnViewCreated ();
            _cachedView.SelectBtn.onClick.AddListener (OnSelectBtn);
        }

        public void SeEmpty () {
            ImageResourceManager.Instance.SetDynamicImageDefault (
                _cachedView.Cover, 
                _cachedView.DefaultProjectCoverTex);
            
            _cachedView.ModifyNum.text = _unkown;
            _cachedView.AddNum.text = _unkown;
            _cachedView.DelNum.text = _unkown;
            _cachedView.PassingRate.text = _unkown;
            _cachedView.SelectMark.SetActive (false);
        }

        public void SetProject (Project project, EChallengeProjectType type, int selectType = 0) {
            _project = project;
            _type = type;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, 
                _project.IconPath,
                _cachedView.DefaultProjectCoverTex);
            _cachedView.AddNum.text = _project.AddCount.ToString();
            _cachedView.ModifyNum.text = _project.ModifyCount.ToString();
            _cachedView.DelNum.text = _project.DeleteCount.ToString();
            _cachedView.PassingRate.text = string.Format("{0}%", _project.PassRate);

            if (1 == selectType) {
                _cachedView.SelectMark.SetActive (true);
            } else {
                _cachedView.SelectMark.SetActive (false);
            }
        }

        public void SetRandomSwitchProject (Project project) {
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, 
                _project.IconPath,
                _cachedView.DefaultProjectCoverTex);
            _cachedView.ModifyNum.text = _unkown;
            _cachedView.AddNum.text = _unkown;
            _cachedView.DelNum.text = _unkown;
            _cachedView.PassingRate.text = _unkown;
        }

        private void OnSelectBtn () {
            Messenger<EChallengeProjectType>.Broadcast (EMessengerType.OnChallengeProjectSelected, _type);
        }
        #endregion

    }
}