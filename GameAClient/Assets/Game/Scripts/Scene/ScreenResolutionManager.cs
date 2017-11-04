using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    /// <summary>
    /// 屏幕分辨率管理器
    /// </summary>
    public class ScreenResolutionManager
    {
        private static ScreenResolutionManager _instance;

        public static ScreenResolutionManager Instance
        {
            get { return _instance ?? (_instance = new ScreenResolutionManager()); }
        }

        private List<Resolution> _allWindowResolutions;
        private List<Resolution> _fullScreenResolutions;
        private Resolution _curResolution;
        private int _curResolutionIndex;
        private bool _fullScreen;

        private const string FullScreenTag = "FullScreenTag";
        private bool _selectFullScreen;

        private int _selectIndex;

        public Resolution CurRealResolution
        {
            get { return _curResolution; }
        }

        public List<Resolution> AllResolutionOptions
        {
            get
            {
                if (_selectFullScreen)
                {
                    return _fullScreenResolutions;
                }
                return _allWindowResolutions;
            }
        }

        public bool FullScreen
        {
            get { return _fullScreen; }
        }

        public int SelectIndex
        {
            get { return _selectIndex; }
        }

        private ScreenResolutionManager()
        {
            GetAllResolutions();
            Load();
            ClearChange();
        }

        private void GetAllResolutions()
        {
            _fullScreenResolutions = new List<Resolution>(1);
            _fullScreenResolutions.Add(Screen.currentResolution);
            _allWindowResolutions = new List<Resolution>(10);
            var resolutions = Screen.resolutions;
            for (int i = 0; i < resolutions.Length; i++)
            {
                int width = resolutions[i].width;
                int height = resolutions[i].height;
                //分辨率高度必须小于屏幕分辨率高度，否则窗口会超出屏幕
                if (width > Screen.currentResolution.width || height >= Screen.currentResolution.height)
                {
                    continue;
                }
                bool canAdd = false;
                if (width / (float) 16 == height / (float) 9 || width / (float) 16 == height / (float) 10)
                {
                    canAdd = true;
                    //检查是否重复
                    for (int j = 0; j < _allWindowResolutions.Count; j++)
                    {
                        if (CheckSameResolution(resolutions[i], _allWindowResolutions[j]))
                        {
                            canAdd = false;
                            break;
                        }
                    }
                }
                if (canAdd)
                {
                    _allWindowResolutions.Add(resolutions[i]);
                }
            }
            _allWindowResolutions.Sort((p, q) => p.width * 1000 + p.height - q.width * 1000 - q.height);
        }

        private void Load()
        {
            if (PlayerPrefs.HasKey(FullScreenTag))
            {
                _fullScreen = PlayerPrefs.GetInt(FullScreenTag) != 0;
            }
            else
            {
                _fullScreen = false;
            }
            _curResolution = new Resolution();
            //设置默认分辨率
            _curResolution.width = Screen.width;
            _curResolution.height = Screen.height;
            _curResolutionIndex = IndexOfResolutions(_curResolution, _allWindowResolutions);
            if (_curResolutionIndex >= 0)
            {
                if (_fullScreen)
                {
                    _curResolutionIndex = 0;
                }
                else
                {
                    _curResolution = _allWindowResolutions[_curResolutionIndex];
                }
            }
            else
            {
                //若列表中没有，则添加
                _allWindowResolutions.Add(_curResolution);
                _allWindowResolutions.Sort((p, q) => p.width * 1000 + p.height - q.width * 1000 - q.height);
                if (_fullScreen)
                {
                    _curResolutionIndex = 0;
                }
                else
                {
                    _curResolutionIndex = _allWindowResolutions.IndexOf(_curResolution);
                }
            }
        }

        public void Save()
        {
            bool needSave = false;
            if (_selectFullScreen != _fullScreen)
            {
                needSave = true;
                _fullScreen = _selectFullScreen;
                PlayerPrefs.SetInt(FullScreenTag, _fullScreen ? 1 : 0);
                if (_selectFullScreen)
                {
                    _curResolution = _fullScreenResolutions[0];
                    _curResolutionIndex = 0;
                }
                else
                {
                    _curResolution = _allWindowResolutions[_selectIndex];
                    _curResolutionIndex = _selectIndex;
                }
            }
            else if (!_selectFullScreen && _selectIndex != _curResolutionIndex)
            {
                needSave = true;
                _curResolutionIndex = _selectIndex;
                _curResolution = _allWindowResolutions[_curResolutionIndex];
            }
            if (needSave)
            {
                SetResolution(_curResolution, _fullScreen);
            }
        }

        public void ClearChange()
        {
            _selectIndex = _curResolutionIndex;
            _selectFullScreen = _fullScreen;
        }

        public void SetFullScreen(bool value)
        {
            _selectFullScreen = value;
            if (_selectFullScreen)
            {
                _selectIndex = 0;
            }
            else
            {
                _selectIndex = _curResolutionIndex;
            }
        }

        public void SetResolution(Resolution resolution, bool fullScreen)
        {
            _curResolution = resolution;
            _fullScreen = fullScreen;
            Screen.SetResolution(resolution.width, resolution.height, fullScreen);
            Screen.fullScreen = fullScreen;
        }

        public void SetResolution(int index)
        {
            if (index >= _allWindowResolutions.Count)
            {
                LogHelper.Error("resolutionIndex > _allResolutions.Count");
                return;
            }
            _selectIndex = index;
//            _curResolutionIndex = index;
//            SetResolution(_allResolutions[index], _fullScreen);
        }

        private bool CheckSameResolution(Resolution r1, Resolution r2)
        {
            return r1.width == r2.width && r1.height == r2.height;
        }

        private int IndexOfResolutions(Resolution r, List<Resolution> resolutions)
        {
            for (int i = 0; i < resolutions.Count; i++)
            {
                if (CheckSameResolution(resolutions[i], r))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}