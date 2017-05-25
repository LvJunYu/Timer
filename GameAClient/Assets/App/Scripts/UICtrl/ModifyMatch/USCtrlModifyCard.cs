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
    public class USCtrlModifyCard : USCtrlBase<USViewModifyCard>
    {
        #region 常量与字段
        #endregion

        #region 属性


        #endregion

        #region 方法
        public override void Init (USViewModifyCard view)
        {
            base.Init (view);
        }

        protected override void OnViewCreated ()
        {
            base.OnViewCreated ();
        }

        public void SeEmpty () {
            _cachedView.Empty.SetActive (true);
            _cachedView.Normal.SetActive (false);
        }

        public void SetProject (Project project, int chapterId, int levelId) {
            _cachedView.Empty.SetActive (false);
            _cachedView.Normal.SetActive (true);

            _cachedView.CanPublish.SetActive (false);
            _cachedView.NotPass.SetActive (false);
            _cachedView.NotModify.SetActive (false);
            if (project.AddCount == 0 && project.DeleteCount == 0 && project.ModifyCount == 0) {
                _cachedView.NotModify.SetActive (true);
            } else if (project.PassFlag){
                _cachedView.CanPublish.SetActive (true);
            } else {
                _cachedView.NotPass.SetActive (true);
            }
            _cachedView.Title.text = string.Format ("第{0}章  第{1}关", chapterId, levelId);

            // 取单人模式的project，因为改造数据中的project可能还没有获得
            Project advProject = null;
            int sectionIdx = LocalUser.Instance.MatchUserData.CurReformSection - 1;
            int levelIdx = LocalUser.Instance.MatchUserData.CurReformLevel - 1;
            if (sectionIdx >= AppData.Instance.AdventureData.ProjectList.SectionList.Count) {
                // todo out of range exception
                return;
            }
            var section = AppData.Instance.AdventureData.ProjectList.SectionList [sectionIdx];
            if (levelIdx >= section.NormalProjectList.Count) {
                // todo out of range exception
                return;
            }
            advProject = section.NormalProjectList [levelIdx];

            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, 
                project.IconPath,
                _cachedView.DefaultProjectCoverTex);
        }
        #endregion

    }
}