using GameA.Game;

namespace GameA
{
    public partial class NetBattleData
    {
        private const string _secondFormat = "{0}秒";
        private const string _minFormat = "{0:f1}分钟";

        public string GetTimeOverCondition()
        {
            switch ((ENetBattleTimeResult) _timeWinCondition)
            {
                case ENetBattleTimeResult.Score:
                    return "分数最高的队伍胜利";
                case ENetBattleTimeResult.AllWin:
                    return "时间到全体胜利";
                case ENetBattleTimeResult.AllFail:
                    return "时间到全体失败";
            }
            return string.Empty;
        }

        public string GetLifeCount()
        {
            if (_infiniteLife)
            {
                return "无限";
            }
            return _lifeCount.ToString();
        }

        public string GetReviveTime()
        {
            return string.Format(_secondFormat, _reviveTime);
        }

        public string GetReviveProtectTime()
        {
            return string.Format(_secondFormat, _reviveInvincibleTime);
        }

        public string GetTimeLimit()
        {
            return string.Format(_minFormat, _timeLimit / (float) 60);
        }
    }
}