
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class USCtrlSocialRelationship : USCtrlBase<USViewSocialRelationship>
    {
        private EResScenary _resScenary;
        private List<UMCtrlRelationCard> _cardList = new List<UMCtrlRelationCard>();
        private RelationshipPage _relationshipPage;
        public RelationshipPage RelationPage
        {
            get
            {
                return _relationshipPage;
            }
            set
            {
                _relationshipPage = value;
            }
        }

        public void Open()
        {
            _cachedView.gameObject.SetActive(true);
        }

        public void Close()
        {
            _cachedView.gameObject.SetActive(false);
        }
        /// <summary>
        /// 枚举分页
        /// </summary>
        public enum RelationshipPage
        {
            Relationshippage1,
            Relationshippage2
        }
        /// <summary>
        /// 根据字符串返回枚举
        /// </summary>
        /// <param name="USUI"></param>
	    public void USUIShopping(RelationshipPage USUI)
        {
            switch (USUI)
            {
                case RelationshipPage.Relationshippage1:
                    break;
                case RelationshipPage.Relationshippage2:
                    break;
            }
        }


        public void Set(List<UserInfoDetail> fittingList, EResScenary resScenary)
        {
            _resScenary = resScenary;
            if (_cardList.Count > 0)
            {
                for (int i = 0; i < _cardList.Count; i++)
                {
                    _cardList[i].Destroy();
                }
            }
            _cardList.Clear();
            for (int i = 0; i < fittingList.Count; i++)
            {
                SetEachCard(fittingList[i]);
            }
            //RefreshPage();
        }

        private void SetEachCard(UserInfoDetail fittingFashion)
        {
            if (_cachedView != null)
            {
                var UM = new UMCtrlRelationCard();
                UM.Init(_cachedView.Dock as RectTransform, _resScenary);
                UM.Set(fittingFashion);
                _cardList.Add(UM);
            }
        }

    }
}

