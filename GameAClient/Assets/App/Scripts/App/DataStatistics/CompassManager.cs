using System.Collections;
using System.Text;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class CompassManager
    {
        public static readonly CompassManager Instance = new CompassManager();
        private const string _appId = "1106419259";
        private const string _version = "1"; //版本号 若未上报，则自动补齐为：1
        private const string _domain = "10"; //QQGame PC:10  IOS:18  Android:19
        private const int _reportOnlineInterval = 300; //游戏每5分钟上报一次在线数据
        private float _lastReportOnlineTime;
        StringBuilder url = new StringBuilder(128);
        private string _userGuid;
        private string _openId;
        private bool _hasInited;

        private CompassManager()
        {
        }

        private bool Init()
        {
            if (_hasInited) return true;
            _userGuid = "&opuid=" + LocalUser.Instance.UserGuid;
            var channel = ChannelQQGame.Instance as ChannelQQGame;
            if (channel == null)
            {
                return false;
            }
            _openId = "&opopenid=" + channel.OpenId;
            _hasInited = true;
            return true;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="opuid">UID为应用自身的帐号体系中用户的ID，例如A偷了B的菜，这里填A的UID。</param>
        /// <param name="opopenid">操作者的OpenID，OpenID为与QQ号码一一对应的字符串，例如A偷了B的菜，这里填A的OpenID</param>
        /// <param name="source">用户在进行登录或者注册等操作的外部来源。例如从某个广告位带来的跳转，填入ad1。填入此字段可以分析到用户登录的来源。推荐填</param>
        /// <param name="userip">用户机器的ip地址，使用主机字节序。若未上报，则自动补齐为：http请求头里的客户机ip</param>
        /// <param name="svrip">这里的IP为当前处理用户请求的机器(cgi或者是server)IP,。若未上报，则自动补齐为：该cgi所在server的ip</param>
        /// <param name="time">当前用户的操作时间，精确到秒，填入unix时间戳。若未上报，则自动补齐为：服务器当前时间</param>
        /// <param name="worldid">不进行分区分服就填1。若未上报，则自动补齐为：1</param>
        /// <param name="level">操作用户的等级，即opuid的等级</param>
        public void Login(string opuid = null, string opopenid = null, string source = null, string userip = null,
            string svrip = null, string time = null, string worldid = null, string level = null)
        {
            if (!Init()) return;
            url.Length = 0;
            url.Append("http://tencentlog.com/stat/report_login.php?")
                .Append(string.Format("appid={0}", _appId))
                .Append(userip == null ? string.Empty : string.Format("&userip={0}", userip))
                .Append(svrip == null ? string.Empty : string.Format("&svrip={0}", svrip))
                .Append(time == null ? string.Empty : string.Format("&time={0}", time))
                .Append(string.Format("&domain={0}", _domain))
                .Append(worldid == null ? string.Empty : string.Format("&worldid={0}", worldid))
                .Append(opuid == null ? _userGuid : string.Format("&opuid={0}", opuid))
                .Append(opopenid == null ? _openId : string.Format("&opopenid={0}", opopenid))
                .Append(source == null ? string.Empty : string.Format("&source={0}", source))
                .Append(level == null ? string.Empty : string.Format("&level={0}", level));
            LogHelper.Info("Login Url: " + url);
            WWW www = new WWW(url.ToString());
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        /// <summary>
        /// 注册
        /// </summary>
        public void Register(string opuid = null, string opopenid = null, string source = null, string userip = null,
            string svrip = null, string time = null, string worldid = null)
        {
            if (!Init()) return;
            url.Length = 0;
            url.Append("http://tencentlog.com/stat/report_register.php?")
                .Append(string.Format("version={0}", _version))
                .Append(string.Format("&appid={0}", _appId))
                .Append(userip == null ? string.Empty : string.Format("&userip={0}", userip))
                .Append(svrip == null ? string.Empty : string.Format("&svrip={0}", svrip))
                .Append(time == null ? string.Empty : string.Format("&time={0}", time))
                .Append(string.Format("&domain={0}", _domain))
                .Append(worldid == null ? string.Empty : string.Format("&worldid={0}", worldid))
                .Append(opuid == null ? _userGuid : string.Format("&opuid={0}", opuid))
                .Append(opopenid == null ? _openId : string.Format("&opopenid={0}", opopenid))
                .Append(source == null ? string.Empty : string.Format("&source={0}", source));
            LogHelper.Info("Register Url: " + url);
            WWW www = new WWW(url.ToString());
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="onlinetime">用户本次登录的在线时长</param>
        public void Quit(string onlinetime, string opuid = null, string opopenid = null, string source = null, string
            userip = null, string svrip = null, string time = null, string worldid = null, string level = null)
        {
            if (!Init()) return;
            url.Length = 0;
            url.Append("http://tencentlog.com/stat/report_quit.php?")
                .Append(string.Format("version={0}", _version))
                .Append(string.Format("&appid={0}", _appId))
                .Append(userip == null ? string.Empty : string.Format("&userip={0}", userip))
                .Append(svrip == null ? string.Empty : string.Format("&svrip={0}", svrip))
                .Append(time == null ? string.Empty : string.Format("&time={0}", time))
                .Append(string.Format("&domain={0}", _domain))
                .Append(worldid == null ? string.Empty : string.Format("&worldid={0}", worldid))
                .Append(opuid == null ? _userGuid : string.Format("&opuid={0}", opuid))
                .Append(opopenid == null ? _openId : string.Format("&opopenid={0}", opopenid))
                .Append(string.Format("&onlinetime={0}", onlinetime))
                .Append(source == null ? string.Empty : string.Format("&source={0}", source))
                .Append(level == null ? string.Empty : string.Format("&level={0}", level));
            LogHelper.Info("Quit Url: " + url);
            new WWW(url.ToString());
//            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        /// <summary>
        /// 实时在线
        /// </summary>
        /// <param name="onlinetime">用户本次登录的在线时长</param>
        private void Online(string user_num = null, string userip = null, string svrip = null, string time = null,
            string worldid = null, string level = null)
        {
            if (!Init()) return;
            url.Length = 0;
            url.Append("http://tencentlog.com/stat/report_online.php?")
                .Append(string.Format("version={0}", _version))
                .Append(string.Format("&appid={0}", _appId))
                .Append(userip == null ? string.Empty : string.Format("&userip={0}", userip))
                .Append(svrip == null ? string.Empty : string.Format("&svrip={0}", svrip))
                .Append(time == null ? string.Empty : string.Format("&time={0}", time))
                .Append(string.Format("&domain={0}", _domain))
                .Append(worldid == null ? string.Empty : string.Format("&worldid={0}", worldid))
                .Append(user_num == null ? "&user_num=1" : string.Format("&user_num={0}", user_num));
            LogHelper.Info("Online Url: " + url);
            WWW www = new WWW(url.ToString());
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        /// <summary>
        /// 用户通过Q点/Q币直接购买游戏内商品的行为；或用户通过游戏内的等值货币“点券/金币/元宝”等来购买游戏内商品的行为。
        /// </summary>
        /// <param name="modifyfee">用户进行操作后，游戏币的变化值。上报单位为Q分（100Q分 = 10Q点 = 1Q币）。如果没有变化，则填0</param>
        /// <param name="totalfee">用户进行操作后，游戏币的总量</param>
        /// <param name="itemid">用户操作物品的ID</param>
        /// <param name="itemtype">用户操作物品ID的分类</param>
        /// <param name="itemcnt">用户操作物品的数量</param>
        /// <param name="modifyexp">用户进行操作后，经验值的变化值</param>
        /// <param name="totalexp">用户进行操作后，经验值的总量</param>
        /// <param name="modifycoin">用户进行操作后，游戏虚拟金币的变化值</param>
        /// <param name="totalcoin">用户进行操作后，虚拟金币的总量</param>
        /* 行为及换算举例
         (1)某用户通过Q币直购游戏内商品，消费10Q币，则记入1000。
         (2)某用户通过点券(游戏币)购买游戏内商品，消费10点券(1Q币=10点券)，则记入100。
        */
        public void Consume(string modifyfee, string totalfee = null, string opuid = null, string opopenid = null,
            string itemid = null, string itemtype = null, string itemcnt = null, string modifyexp = null,
            string totalexp = null, string modifycoin = null, string totalcoin = null, string level = null,
            string source = null, string userip = null, string svrip = null, string time = null, string worldid = null)
        {
            if (!Init()) return;
            url.Length = 0;
            url.Append("http://tencentlog.com/stat/report_consume.php?")
                .Append(string.Format("version={0}", _version))
                .Append(string.Format("&appid={0}", _appId))
                .Append(userip == null ? string.Empty : string.Format("&userip={0}", userip))
                .Append(svrip == null ? string.Empty : string.Format("&svrip={0}", svrip))
                .Append(time == null ? string.Empty : string.Format("&time={0}", time))
                .Append(string.Format("&domain={0}", _domain))
                .Append(worldid == null ? string.Empty : string.Format("&worldid={0}", worldid))
                .Append(opuid == null ? _userGuid : string.Format("&opuid={0}", opuid))
                .Append(opopenid == null ? _openId : string.Format("&opopenid={0}", opopenid))
                .Append(string.Format("&modifyfee={0}", modifyfee))
                .Append(totalfee == null ? string.Empty : string.Format("&totalfee={0}", totalfee))
                .Append(itemid == null ? string.Empty : string.Format("&itemid={0}", itemid))
                .Append(itemtype == null ? string.Empty : string.Format("&itemtype={0}", itemtype))
                .Append(itemcnt == null ? string.Empty : string.Format("&itemcnt={0}", itemcnt))
                .Append(modifyexp == null ? string.Empty : string.Format("&modifyexp={0}", modifyexp))
                .Append(totalexp == null ? string.Empty : string.Format("&totalexp={0}", totalexp))
                .Append(modifycoin == null ? string.Empty : string.Format("&modifycoin={0}", modifycoin))
                .Append(totalfee == null ? string.Empty : string.Format("&totalfee={0}", totalfee))
                .Append(totalcoin == null ? string.Empty : string.Format("&totalcoin={0}", totalcoin))
                .Append(level == null ? string.Empty : string.Format("&level={0}", level))
                .Append(source == null ? string.Empty : string.Format("&source={0}", source));
            LogHelper.Info("Consume Url: " + url);
            WWW www = new WWW(url.ToString());
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        /// <summary>
        /// 用户通过Q点/Q币兑换游戏内等值货币(例如“点券/金币/元宝”)的行为。
        /// </summary>
        public void Recharge(string modifyfee, string totalfee = null, string opuid = null, string opopenid = null,
            string itemid = null, string itemtype = null, string itemcnt = null, string modifyexp = null,
            string totalexp = null, string modifycoin = null, string totalcoin = null, string level = null,
            string source = null, string userip = null, string svrip = null, string time = null, string worldid = null)
        {
            if (!Init()) return;
            url.Length = 0;
            url.Append("http://tencentlog.com/stat/report_recharge.php?")
                .Append(string.Format("version={0}", _version))
                .Append(string.Format("&appid={0}", _appId))
                .Append(userip == null ? string.Empty : string.Format("&userip={0}", userip))
                .Append(svrip == null ? string.Empty : string.Format("&svrip={0}", svrip))
                .Append(time == null ? string.Empty : string.Format("&time={0}", time))
                .Append(string.Format("&domain={0}", _domain))
                .Append(worldid == null ? string.Empty : string.Format("&worldid={0}", worldid))
                .Append(string.Format("&opuid={0}", opuid))
                .Append(opuid == null ? _userGuid : string.Format("&opuid={0}", opuid))
                .Append(opopenid == null ? _openId : string.Format("&opopenid={0}", opopenid))
                .Append(totalfee == null ? string.Empty : string.Format("&totalfee={0}", totalfee))
                .Append(itemid == null ? string.Empty : string.Format("&itemid={0}", itemid))
                .Append(itemtype == null ? string.Empty : string.Format("&itemtype={0}", itemtype))
                .Append(itemcnt == null ? string.Empty : string.Format("&itemcnt={0}", itemcnt))
                .Append(modifyexp == null ? string.Empty : string.Format("&modifyexp={0}", modifyexp))
                .Append(totalexp == null ? string.Empty : string.Format("&totalexp={0}", totalexp))
                .Append(modifycoin == null ? string.Empty : string.Format("&modifycoin={0}", modifycoin))
                .Append(totalfee == null ? string.Empty : string.Format("&totalfee={0}", totalfee))
                .Append(totalcoin == null ? string.Empty : string.Format("&totalcoin={0}", totalcoin))
                .Append(level == null ? string.Empty : string.Format("&level={0}", level))
                .Append(source == null ? string.Empty : string.Format("&source={0}", source));
            LogHelper.Info("Recharge Url: " + url);
            WWW www = new WWW(url.ToString());
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        /// <summary>
        /// 上报行为
        /// </summary>
        /// <param name="optype">支付类操作为1；留言发表类为2；写操作类为3；读操作类为4；其它为5</param>
        /// <param name="actionid">操作ID。1~100为保留字段</param>
        /* 
             1~100为保留字段，其中：
            登录为1；
            主动注册为2；
            接受邀请注册为3；
            邀请他人注册是4；
            支付消费为5。
            留言为6；
            留言回复为7；
            如有其它类型的留言发表类操作，请填8。
            用户登出为9；
            角色登录为11；
            创建角色为12；
            角色退出为13；
            角色实时在线为14。
            支付充值为15。
            其它操作请开发者使用100以上的int型数据上报。
         */
        public void Report(string optype, string actionid, string opuid = null, string opopenid = null,
            string userip = null, string svrip = null, string time = null, string worldid = null)
        {
            if (!Init()) return;
            url.Length = 0;
            url.Append("http://tencentlog.com/stat/report.php?")
                .Append(string.Format("optype={0}", optype))
                .Append(string.Format("actionid={0}", actionid))
                .Append(string.Format("version={0}", _version))
                .Append(string.Format("appid={0}", _appId))
                .Append(userip == null ? string.Empty : string.Format("&userip={0}", userip))
                .Append(svrip == null ? string.Empty : string.Format("&svrip={0}", svrip))
                .Append(time == null ? string.Empty : string.Format("&time={0}", time))
                .Append(string.Format("&domain={0}", _domain))
                .Append(worldid == null ? string.Empty : string.Format("&worldid={0}", worldid))
                .Append(opuid == null ? _userGuid : string.Format("&opuid={0}", opuid))
                .Append(opopenid == null ? _openId : string.Format("&opopenid={0}", opopenid));
            LogHelper.Info("Login Url: " + url);
            WWW www = new WWW(url.ToString());
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        public void Update()
        {
            if (Time.realtimeSinceStartup - _lastReportOnlineTime > _reportOnlineInterval)
            {
                Online();
                _lastReportOnlineTime += _reportOnlineInterval;
            }
        }

        private IEnumerator GetResult(WWW www)
        {
            yield return www;
            LogHelper.Error(www.text);
        }
    }
}