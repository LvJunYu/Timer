using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    public partial class SuspensionTip :MonoBehaviour ,IPointerEnterHandler,IPointerExitHandler
    {
        public GameObject TipGameObject;
        private UMCtrlTip _tip;

        private void Start()
        {
            TipGameObject.SetActiveEx(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TipGameObject.SetActiveEx(true);
//            if (_tip!=null)
//            {
//                return;
//            }
//          _tip =    TipPool.Instance.GetTip(this.rectTransform(), Vector3.zero, this.TipText.text); 
//            throw new System.NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TipGameObject.SetActive(false);
//            if (_tip!=null)
//            {
//                TipPool.Instance.DisposTip(_tip);
//            }
//            throw new System.NotImplementedException();
        }
    }
}