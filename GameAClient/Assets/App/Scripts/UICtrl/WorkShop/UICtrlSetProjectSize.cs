using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlSetProjectSize : UICtrlGenericBase<UIViewSetProjectSize>
    {
        #region Fields
        private static IntVec2 DefaultSelectedIdx = new IntVec2(2, 1);
        private static int[] HorizontalSizes = {20,30,40,50,60};
        private static int[] VerticalSizes = {10,20,30};
        private IntVec2 _selectedIdx;
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen (object parameter)
        {
            base.OnOpen(parameter);

            _selectedIdx = DefaultSelectedIdx;
            RefreshView ();
        }
        
        protected override void OnClose() {
            
            base.OnClose ();
        }
        
        protected override void InitEventListener() {
            base.InitEventListener ();
        }
        
        protected override void OnViewCreated() {
            base.OnViewCreated ();

            _cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);
            _cachedView.OKBtn.onClick.AddListener (OnOKBtn);

            for (int y = 0; y < VerticalSizes.Length; y++) {
                for (int x = 0; x < HorizontalSizes.Length; x++) {
                    int xx = x;
                    int yy = y;
                    _cachedView.Btns [x * VerticalSizes.Length + y].onClick.AddListener (() => {
                        OnClickXY(xx, yy);
                    });
                }
            }
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        private void RefreshView () {
            for (int i = 0; i < _cachedView.SelectedMarkHorizontal.Length; i++) {
                if (i == _selectedIdx.x) {
                    _cachedView.SelectedMarkHorizontal [i].SetActive (true);
                } else {
                    _cachedView.SelectedMarkHorizontal [i].SetActive (false);
                }
            }
            for (int i = 0; i < _cachedView.SelectedMarkVertical.Length; i++) {
                if (i == _selectedIdx.y) {
                    _cachedView.SelectedMarkVertical [i].SetActive (true);
                } else {
                    _cachedView.SelectedMarkVertical [i].SetActive (false);
                }
            }
            int rectIdx = _selectedIdx.x * VerticalSizes.Length + _selectedIdx.y;
            for (int i = 0; i < _cachedView.Rects.Length; i++) {
                if (i == rectIdx) {
                    _cachedView.Rects [i].SetActive (true);
                } else {
                    _cachedView.Rects [i].SetActive (false);
                }
            }
        }

        private void OnClickXY (int x, int y)
        {
            _selectedIdx.x = x;
            _selectedIdx.y = y;
            RefreshView ();
        }

        private void OnOKBtn () {
            Game.MapManager.Instance.SetDefaultMapSize (new IntVec2(HorizontalSizes[_selectedIdx.x], VerticalSizes[_selectedIdx.y]));
            ProcessCreate ();
            SocialGUIManager.Instance.CloseUI<UICtrlSetProjectSize> ();
        }

        private void OnCloseBtn () {
            SocialGUIManager.Instance.CloseUI<UICtrlSetProjectSize> ();
        }

        private void ProcessCreate()
        {
            //            var userMatrixData = AppData.Instance.UserMatrixData.GetData(_content.MatrixGuid);
            //            int localCount = LocalUser.Instance.User.GetSavedProjectCount();
            //            if(userMatrixData.PersonalProjectWorkshopSize <= localCount)
            //            {
            //                CommonTools.ShowPopupDialog("工坊已满，请升级匠人等级或者前去工坊整理");
            //                return;
            //            }

            EMatrixProjectResState resState = EMatrixProjectResState.None;
            if(!MatrixProjectTools.CheckMatrixStateForRun(out resState))
            {
                MatrixProjectTools.ShowMatrixProjectResCheckTip(resState);
                return;
            }
            float needDownloadSize = LocalResourceManager.Instance.GetNeedDownloadSizeMB("GameMaker2D");
            if(Application.internetReachability != NetworkReachability.NotReachable
                && !Util.IsFloatEqual(needDownloadSize, 0))
            {
                CommonTools.ShowPopupDialog(string.Format("本次进入游戏需要更新 {0:N2}MB 资源，可能会产生费用，是否继续？", Mathf.Max(needDownloadSize, 0.01f)),
                    null,
                    new System.Collections.Generic.KeyValuePair<string, Action>("继续", ()=>{
                        InternalCreate();
                    }),
                    new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
                        LogHelper.Debug("Cancel RunCreate");
                    })
                );
            }
            else
            {
                InternalCreate();
            }
        }
        private void InternalCreate()
        {
            Project project = Project.CreateWorkShopProject();
            MatrixProjectTools.SetProjectVersion(project);
            GameManager.Instance.RequestCreate (project);
            SocialGUIManager.Instance.ChangeToGameMode();
        }

        #endregion
    }
}
