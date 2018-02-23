using System.Collections;
using System.Text;
using SoyEngine;
using SoyEngine.LitJson;
using UnityEngine;

namespace GameA
{
    public class ReYunManager
    {
        public static readonly ReYunManager Instance = new ReYunManager();
        private readonly string _deviceId;
        private readonly string _appId;
        private string _userId;
        private const int _heartbeatInterval = 300; //游戏每5分钟上报一次在线
        private float _lastHeartbeatTime;
        private bool _hasInited;
        private const string server = "http://log.reyun.com";
        private WWWForm _wwwForm = new WWWForm();

        private ReYunManager()
        {
            _deviceId = SystemInfo.graphicsDeviceID.ToString();
            _appId = "73a5016064d78de3f8cd01781d923496";
            _wwwForm.headers["Content-Type"] = "application/json";
        }

        public void Init()
        {
            if (SocialApp.Instance.Env != EEnvironment.Production) return;
            _userId = LocalUser.Instance.UserGuid.ToString();
            _hasInited = true;
        }

        public void Install()
        {
            var url = server + "/receive/rest/install";
            JsonData data = new JsonData();
            data["appid"] = _appId;
            data["context"] = new JsonData();
            data["context"]["deviceid"] = _deviceId;
            WWW www = new WWW(url, Encoding.UTF8.GetBytes(data.ToJson()), _wwwForm.headers);
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        public void Startup()
        {
            var url = server + "/receive/rest/startup";
            JsonData data = new JsonData();
            data["appid"] = _appId;
            data["context"] = new JsonData();
            data["context"]["deviceid"] = _deviceId;
            WWW www = new WWW(url, Encoding.UTF8.GetBytes(data.ToJson()), _wwwForm.headers);
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        public void Register()
        {
            if (!_hasInited) return;
            var url = server + "/receive/rest/register";
            JsonData data = new JsonData();
            data["appid"] = _appId;
            data["who"] = _userId;
            data["context"] = new JsonData();
            data["context"]["deviceid"] = _deviceId;
            WWW www = new WWW(url, Encoding.UTF8.GetBytes(data.ToJson()), _wwwForm.headers);
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        public void Login()
        {
            if (!_hasInited) return;
            var url = server + "/receive/rest/loggedin";
            JsonData data = new JsonData();
            data["appid"] = _appId;
            data["who"] = _userId;
            data["context"] = new JsonData();
            data["context"]["deviceid"] = _deviceId;
            WWW www = new WWW(url, Encoding.UTF8.GetBytes(data.ToJson()), _wwwForm.headers);
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        public void Quit(int onlineTime)
        {
            if (!_hasInited) return;
            var url = server + "/receive/rest/event";
            JsonData data = new JsonData();
            data["appid"] = _appId;
            data["who"] = _userId;
            data["what"] = _userId;
            data["context"] = new JsonData();
            data["context"]["deviceid"] = _deviceId;
            data["context"]["onlineTime"] = onlineTime.ToString();
            new WWW(url, Encoding.UTF8.GetBytes(data.ToJson()), _wwwForm.headers);
        }

        private void Heartbeat()
        {
            if (!_hasInited) return;
            var url = server + "/receive/rest/heartbeat";
            JsonData data = new JsonData();
            data["appid"] = _appId;
            data["who"] = _userId;
            data["context"] = new JsonData();
            data["context"]["deviceid"] = _deviceId;
            WWW www = new WWW(url, Encoding.UTF8.GetBytes(data.ToJson()), _wwwForm.headers);
            CoroutineProxy.Instance.StartCoroutine(GetResult(www));
        }

        public void Update()
        {
            if (!_hasInited) return;
            if (Time.realtimeSinceStartup - _lastHeartbeatTime > _heartbeatInterval)
            {
                Heartbeat();
                _lastHeartbeatTime += _heartbeatInterval;
            }
        }

        private IEnumerator GetResult(WWW www)
        {
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                LogHelper.Info(www.text);
            }
            else
            {
                LogHelper.Error(www.error);
            }
        }
    }
}