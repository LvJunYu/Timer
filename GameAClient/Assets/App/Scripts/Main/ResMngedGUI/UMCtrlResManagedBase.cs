using SoyEngine;
using UnityEngine;

/*UM 是啥，初始化*/
namespace GameA
{
    public class UMCtrlResManagedBase<T> : UMCtrlGenericBase<T> where T : UMViewBase
    {
        #region 变量
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected virtual bool Init(RectTransform parent, EResScenary resScenary, Vector3 localpos, ResManagedUIRoot uiRoot)
        {
            if (string.IsNullOrEmpty(_prefabName))
            {
                LogHelper.Error("_prefabName is nullOrEmpty");
                return false;
            }
            if (parent == null)
            {
                LogHelper.Error("parent == null");
                return false;
            }
            _view = uiRoot.InstanceItemView(_prefabName, resScenary);
            if (_view == null)
                return false;
            if (!_view.gameObject.activeSelf)
                _view.gameObject.SetActive(true);
            CommonTools.SetParent(_view.transform, parent);
            _view.Init();
            _view.Trans.anchoredPosition = localpos;
            _isViewCreated = true;
            OnViewCreated();
            return true;
        }

        #endregion
    }
}