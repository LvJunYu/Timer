using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlPersonalInfoBasicInfo : UPCtrlPersonalInfoBase
    {
        private static string _expFormat = "{0}/{1}";
        private static string _lvFormat = "Lv{0}";
        private UserWorldProjectPlayHistoryList _data;
        private UserInfoDetail _userInfoDetail;

        public override void Open()
        {
            base.Open();
            RefreshView();
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            _userInfoDetail = _mainCtrl.UserInfoDetail;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg,
                _userInfoDetail.UserInfoSimple.HeadImgUrl, _cachedView.HeadDefaltTexture);
            _cachedView.Name.text = _userInfoDetail.UserInfoSimple.NickName;
            _cachedView.Desc.text = _userInfoDetail.Profile;
            _cachedView.FollowNum.text = _userInfoDetail.RelationStatistic.FollowCount.ToString();
            _cachedView.FansNum.text = _userInfoDetail.RelationStatistic.FollowerCount.ToString();
            _cachedView.MaleObj.SetActive(_userInfoDetail.UserInfoSimple.Sex == ESex.S_Male);
            _cachedView.FamaleObj.SetActive(_userInfoDetail.UserInfoSimple.Sex == ESex.S_Female);
            int advLv = _userInfoDetail.UserInfoSimple.LevelData.PlayerLevel;
            int createLv = _userInfoDetail.UserInfoSimple.LevelData.CreatorLevel;
            long curAdvExp = _userInfoDetail.UserInfoSimple.LevelData.PlayerExp;
            long curCreateExp = _userInfoDetail.UserInfoSimple.LevelData.CreatorExp;
            long needAdvExp = TableManager.Instance.GetPlayerLvToExp(advLv + 1) != null
                ? TableManager.Instance.GetPlayerLvToExp(advLv + 1).AdvExp
                : curAdvExp;
            long needCreateExp = TableManager.Instance.GetPlayerLvToExp(createLv + 1) != null
                ? TableManager.Instance.GetPlayerLvToExp(createLv + 1).MakerExp
                : curCreateExp;
            _cachedView.AdvLv.text = string.Format(_lvFormat, advLv);
            _cachedView.CreateLv.text = string.Format(_lvFormat, createLv);
            _cachedView.AdvExp.text = string.Format(_expFormat, curAdvExp, needAdvExp);
            _cachedView.CreateExp.text = string.Format(_expFormat, curCreateExp, needCreateExp);
            _cachedView.AdvExpBar.fillAmount = curAdvExp / (float) needAdvExp;
            _cachedView.CreateExpBar.fillAmount = curCreateExp / (float) needCreateExp;
        }
    }
}