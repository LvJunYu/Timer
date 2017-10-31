using System.Collections;
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
        private double _timeOut;

        private CompassManager()
        {
        }

        protected IEnumerator GetResult(WWW www)
        {
            yield return www;
            LogHelper.Info(www.text);
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
        public void Login(string opuid, string opopenid, string source = null, string userip = null,
            string svrip = null, string time = null, string worldid = null, string level = null)
        {
            string url = "http://tencentlog.com/stat/report_login.php?";
            url += string.Format("appid={0}", _appId);
            url += userip == null ? string.Empty : string.Format("&userip={0}", userip);
            url += svrip == null ? string.Empty : string.Format("&svrip={0}", svrip);
            url += time == null ? string.Empty : string.Format("&time={0}", time);
            url += string.Format("&domain={0}", _domain);
            url += worldid == null ? string.Empty : string.Format("&worldid={0}", worldid);
            url += string.Format("&opuid={0}", opuid);
            url += string.Format("&opopenid={0}", opopenid);
            url += source == null ? string.Empty : string.Format("&source={0}", source);
            url += level == null ? string.Empty : string.Format("&level={0}", level);
            LogHelper.Info("Login Url: " + url);
            WWW www = new WWW(url);
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        /// <summary>
        /// 注册
        /// </summary>
        public void Register(string opuid, string opopenid, string source = null, string userip = null,
            string svrip = null, string time = null, string worldid = null)
        {
            string url = "http://tencentlog.com/stat/report_register.php?";
            url += string.Format("version={0}", _version);
            url += string.Format("&appid={0}", _appId);
            url += userip == null ? string.Empty : string.Format("&userip={0}", userip);
            url += svrip == null ? string.Empty : string.Format("&svrip={0}", svrip);
            url += time == null ? string.Empty : string.Format("&time={0}", time);
            url += string.Format("&domain={0}", _domain);
            url += worldid == null ? string.Empty : string.Format("&worldid={0}", worldid);
            url += string.Format("&opuid={0}", opuid);
            url += string.Format("&opopenid={0}", opopenid);
            url += source == null ? string.Empty : string.Format("&source={0}", source);
            LogHelper.Info("Register Url: " + url);
            WWW www = new WWW(url);
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="onlinetime">用户本次登录的在线时长</param>
        public void Quit(string opuid, string opopenid, string onlinetime, string source = null, string userip = null,
            string svrip = null, string time = null, string worldid = null, string level = null)
        {
            string url = "http://tencentlog.com/stat/report_quit.php?";
            url += string.Format("version={0}", _version);
            url += string.Format("&appid={0}", _appId);
            url += userip == null ? string.Empty : string.Format("&userip={0}", userip);
            url += svrip == null ? string.Empty : string.Format("&svrip={0}", svrip);
            url += time == null ? string.Empty : string.Format("&time={0}", time);
            url += string.Format("&domain={0}", _domain);
            url += worldid == null ? string.Empty : string.Format("&worldid={0}", worldid);
            url += string.Format("&opuid={0}", opuid);
            url += string.Format("&opopenid={0}", opopenid);
            url += string.Format("&onlinetime={0}", onlinetime);
            url += source == null ? string.Empty : string.Format("&source={0}", source);
            url += level == null ? string.Empty : string.Format("&level={0}", level);
            LogHelper.Info("Login Url: " + url);
            WWW www = new WWW(url);
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        /// <summary>
        /// 实时在线
        /// </summary>
        /// <param name="onlinetime">用户本次登录的在线时长</param>
        public void Online(string user_num = null, string userip = null, string svrip = null, string time = null,
            string worldid = null, string level = null)
        {
            string url = "http://tencentlog.com/stat/report_online.php?";
            url += string.Format("version={0}", _version);
            url += string.Format("&appid={0}", _appId);
            url += userip == null ? string.Empty : string.Format("&userip={0}", userip);
            url += svrip == null ? string.Empty : string.Format("&svrip={0}", svrip);
            url += time == null ? string.Empty : string.Format("&time={0}", time);
            url += string.Format("&domain={0}", _domain);
            url += worldid == null ? string.Empty : string.Format("&worldid={0}", worldid);
            url += user_num == null ? "&user_num=1" : string.Format("&user_num={0}", user_num);
            LogHelper.Info("Online Url: " + url);
            WWW www = new WWW(url);
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
        public void Consume(string opuid, string opopenid, string modifyfee, string totalfee = null,
            string itemid = null, string itemtype = null, string itemcnt = null, string modifyexp = null,
            string totalexp = null, string modifycoin = null, string totalcoin = null, string level = null,
            string source = null, string userip = null, string svrip = null, string time = null, string worldid = null)
        {
            string url = "http://tencentlog.com/stat/report_consume.php?";
            url += string.Format("version={0}", _version);
            url += string.Format("&appid={0}", _appId);
            url += userip == null ? string.Empty : string.Format("&userip={0}", userip);
            url += svrip == null ? string.Empty : string.Format("&svrip={0}", svrip);
            url += time == null ? string.Empty : string.Format("&time={0}", time);
            url += string.Format("&domain={0}", _domain);
            url += worldid == null ? string.Empty : string.Format("&worldid={0}", worldid);
            url += string.Format("&opuid={0}", opuid);
            url += string.Format("&opopenid={0}", opopenid);
            url += string.Format("&modifyfee={0}", modifyfee);
            url += totalfee == null ? string.Empty : string.Format("&totalfee={0}", totalfee);
            url += itemid == null ? string.Empty : string.Format("&itemid={0}", itemid);
            url += itemtype == null ? string.Empty : string.Format("&itemtype={0}", itemtype);
            url += itemcnt == null ? string.Empty : string.Format("&itemcnt={0}", itemcnt);
            url += modifyexp == null ? string.Empty : string.Format("&modifyexp={0}", modifyexp);
            url += totalexp == null ? string.Empty : string.Format("&totalexp={0}", totalexp);
            url += modifycoin == null ? string.Empty : string.Format("&modifycoin={0}", modifycoin);
            url += totalfee == null ? string.Empty : string.Format("&totalfee={0}", totalfee);
            url += totalcoin == null ? string.Empty : string.Format("&totalcoin={0}", totalcoin);
            url += level == null ? string.Empty : string.Format("&level={0}", level);
            url += source == null ? string.Empty : string.Format("&source={0}", source);
            LogHelper.Info("Consume Url: " + url);
            WWW www = new WWW(url);
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        /// <summary>
        /// 用户通过Q点/Q币兑换游戏内等值货币(例如“点券/金币/元宝”)的行为。
        /// </summary>
        public void Recharge(string opuid, string opopenid, string modifyfee, string totalfee = null,
            string itemid = null, string itemtype = null, string itemcnt = null, string modifyexp = null,
            string totalexp = null, string modifycoin = null, string totalcoin = null, string level = null,
            string source = null, string userip = null, string svrip = null, string time = null, string worldid = null)
        {
            string url = "http://tencentlog.com/stat/report_recharge.php?";
            url += string.Format("version={0}", _version);
            url += string.Format("&appid={0}", _appId);
            url += userip == null ? string.Empty : string.Format("&userip={0}", userip);
            url += svrip == null ? string.Empty : string.Format("&svrip={0}", svrip);
            url += time == null ? string.Empty : string.Format("&time={0}", time);
            url += string.Format("&domain={0}", _domain);
            url += worldid == null ? string.Empty : string.Format("&worldid={0}", worldid);
            url += string.Format("&opuid={0}", opuid);
            url += string.Format("&opopenid={0}", opopenid);
            url += string.Format("&modifyfee={0}", modifyfee);
            url += totalfee == null ? string.Empty : string.Format("&totalfee={0}", totalfee);
            url += itemid == null ? string.Empty : string.Format("&itemid={0}", itemid);
            url += itemtype == null ? string.Empty : string.Format("&itemtype={0}", itemtype);
            url += itemcnt == null ? string.Empty : string.Format("&itemcnt={0}", itemcnt);
            url += modifyexp == null ? string.Empty : string.Format("&modifyexp={0}", modifyexp);
            url += totalexp == null ? string.Empty : string.Format("&totalexp={0}", totalexp);
            url += modifycoin == null ? string.Empty : string.Format("&modifycoin={0}", modifycoin);
            url += totalfee == null ? string.Empty : string.Format("&totalfee={0}", totalfee);
            url += totalcoin == null ? string.Empty : string.Format("&totalcoin={0}", totalcoin);
            url += level == null ? string.Empty : string.Format("&level={0}", level);
            url += source == null ? string.Empty : string.Format("&source={0}", source);
            LogHelper.Info("Recharge Url: " + url);
            WWW www = new WWW(url);
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
        public void Report(string optype, string actionid, string opuid, string opopenid, string userip = null,
            string svrip = null, string time = null, string worldid = null)
        {
            string url = "http://tencentlog.com/stat/report.php?";
            url += string.Format("optype={0}", optype);
            url += string.Format("actionid={0}", actionid);
            url += string.Format("version={0}", _version);
            url += string.Format("appid={0}", _appId);
            url += userip == null ? string.Empty : string.Format("&userip={0}", userip);
            url += svrip == null ? string.Empty : string.Format("&svrip={0}", svrip);
            url += time == null ? string.Empty : string.Format("&time={0}", time);
            url += string.Format("&domain={0}", _domain);
            url += worldid == null ? string.Empty : string.Format("&worldid={0}", worldid);
            url += string.Format("&opuid={0}", opuid);
            url += string.Format("&opopenid={0}", opopenid);
            LogHelper.Info("Login Url: " + url);
            WWW www = new WWW(url);
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }
    }
}