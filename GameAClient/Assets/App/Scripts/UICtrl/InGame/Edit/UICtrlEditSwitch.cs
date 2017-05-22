/********************************************************************
** Filename : UICtrlEdit
** Author : Dong
** Date : 2015/7/2 16:30:13
** Summary : UICtrlEdit
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Create)]
    public class UICtrlEditSwitch : UICtrlGenericBase<UIViewEditSwitch>
    {
        #region 常量与字段

        private static Vector2 TextOffset = new Vector2(0.35f, 0.4f);

        private List<Text> _connectionCntCache = new List<Text> ();
        private List<Button> _delBtnCache = new List<Button> ();

        private List<IntVec3> _allUnitGuids;
        private List<int> _allUnitConnectionCnts = new List<int>();
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

			
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            Messenger<List<Vector3>>.AddListener(EMessengerType.OnSelectedItemChangedOnSwitchMode, OnSelectedItemChanged);
            Messenger<IntVec3, IntVec3, bool>.AddListener (EMessengerType.OnSwitchConnectionChanged, OnSwitchConnectionChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);

            if (null != parameter) {
                _allUnitGuids = parameter as List<IntVec3>;
                _allUnitConnectionCnts.Clear ();
                for (int i = 0; i < _allUnitGuids.Count; i++) {
                    var list = DataScene2D.Instance.GetControlledUnits (_allUnitGuids [i]);
                    if (null != list) {
                        _allUnitConnectionCnts.Add (list.Count);                        
                    } else {
                        var list2 = DataScene2D.Instance.GetSwitchUnitsConnected (_allUnitGuids [i]);
                        if (null != list2) {
                            _allUnitConnectionCnts.Add (list2.Count);
                        }
                    }
                }
                OnCameraPosChanged();
            }

        }

        protected override void OnClose ()
        {
            base.OnClose ();
            OnSelectedItemChanged (null);

            for (int i = 0; i < _connectionCntCache.Count; i++) {
                _connectionCntCache [i].transform.parent.gameObject.SetActive (false);
            }
        }

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
            Messenger<List<Vector3>>.RemoveListener(EMessengerType.OnSelectedItemChangedOnSwitchMode, OnSelectedItemChanged);
            Messenger<IntVec3, IntVec3, bool>.RemoveListener (EMessengerType.OnSwitchConnectionChanged, OnSwitchConnectionChanged);
		}

        public override void OnUpdate ()
        {
            base.OnUpdate ();
        }

        private void OnDelBtn (int idx) {
            EditMode.Instance.DeleteSwitchConnection (idx);
        }

        private void OnCameraPosChanged () {
            if (null != _allUnitGuids) {
                int i = 0;
                for (; i < _allUnitGuids.Count; i++) {
                    Text txt = GetCntText (i);
                    txt.transform.parent.localPosition = GM2DTools.WorldToScreenPoint ((Vector2)GM2DTools.TileToWorld(_allUnitGuids[i]) + TextOffset);
                    txt.transform.parent.gameObject.SetActive (true);
                    if (_allUnitConnectionCnts.Count > i) {
                        txt.text = _allUnitConnectionCnts [i].ToString ();
                    }
                }
                for (; i < _connectionCntCache.Count; i++) {
                    _connectionCntCache[i].transform.parent.gameObject.SetActive (false);
                }
            }
        }

        private void OnSelectedItemChanged (List<Vector3> lineCenters) {
            if (null == lineCenters) {
                for (int j = 0; j < _delBtnCache.Count; j++) {
                    _delBtnCache [j].gameObject.SetActive (false);
                }
                return;
            }
            int i = 0;
            for (; i < lineCenters.Count; i++) {
                Button btn = GetDelBtn (i);
                btn.transform.localPosition = GM2DTools.WorldToScreenPoint ((Vector2)lineCenters[i]);
                btn.gameObject.SetActive (true);
            }
            for (; i < _delBtnCache.Count; i++) {
                _delBtnCache[i].gameObject.SetActive (false);
            }
        }

        private void OnSwitchConnectionChanged (IntVec3 a, IntVec3 b, bool isAdd) {
            int found = 0;
            for (int i = 0; i < _allUnitGuids.Count; i++) {
                if (_allUnitGuids [i] == a || _allUnitGuids [i] == b) {
                    if (_allUnitConnectionCnts.Count > i) {
                        _allUnitConnectionCnts [i] += isAdd ? 1 : -1;
                        if (_connectionCntCache.Count > i) {
                            _connectionCntCache [i].text = _allUnitConnectionCnts [i].ToString ();
                        }
                        found++;
                        if (found >= 2) {
                            break;
                        }
                    }
                }
            }
        }

        private Button GetDelBtn (int idx) {
            while (_delBtnCache.Count <= idx) {
                GameObject newObj = GameObject.Instantiate (_cachedView.LineDelBtnPrefb.gameObject, _cachedView.LineDelBtnPrefb.transform.parent);
                Button newBtn = newObj.GetComponent<Button> ();
                _delBtnCache.Add (newBtn);
            }
            _delBtnCache [idx].onClick.RemoveAllListeners ();
            _delBtnCache [idx].onClick.AddListener (() => OnDelBtn (idx));
            return _delBtnCache [idx];
        }

        private Text GetCntText (int idx) {
            while (_connectionCntCache.Count <= idx) {
                GameObject newObj = GameObject.Instantiate (_cachedView.ConnectionCntPrefb.gameObject, _cachedView.ConnectionCntPrefb.transform.parent);
                Text newTxt = newObj.GetComponentInChildren<Text> ();
                _connectionCntCache.Add (newTxt);
            }
            return _connectionCntCache [idx];
        }
		#endregion
	}
}
