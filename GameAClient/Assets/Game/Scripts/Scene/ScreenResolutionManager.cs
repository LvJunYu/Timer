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

        private List<Resolution> _allResolutions;
        private Resolution _curResolution;
        private int _curResolutionIndex;
        private bool _fullScreen;
        private const string FullScreenTag = "FullScreenTag";
        private const string ScreenWidthTag = "ScreenWidthTag";
        private const string ScreenHeightTag = "ScreenHeightTag";
        private bool _selectFullScreen;
        private int _selectIndex;

        public Resolution CurRealResolution
        {
            get
            {
                if (_fullScreen)
                {
                    return Screen.currentResolution;
                }
                return _curResolution;
            }
        }

        public List<Resolution> AllResolutions
        {
            get { return _allResolutions; }
        }

        public int CurResolutionIndex
        {
            get { return _curResolutionIndex; }
        }

        public bool FullScreen
        {
            get { return _fullScreen; }
        }

        private ScreenResolutionManager()
        {
            GetAllResolutions();
            Load();
            ClearChange();
        }

        private void GetAllResolutions()
        {
            _allResolutions = new List<Resolution>(10);
            var resolutions = Screen.resolutions;
            for (int i = 0; i < resolutions.Length; i++)
            {
                int width = resolutions[i].width;
                int height = resolutions[i].height;
                if (width > Screen.currentResolution.width || height > Screen.currentResolution.height)
                {
                    continue;
                }
                bool canAdd = false;
                if (width / (float) 16 == height / (float) 9 || width / (float) 16 == height / (float) 10)
                {
                    canAdd = true;
                    //检查是否重复
                    for (int j = 0; j < _allResolutions.Count; j++)
                    {
                        if (CheckSameResolution(resolutions[i], _allResolutions[j]))
                        {
                            canAdd = false;
                            break;
                        }
                    }
                }
                if (canAdd)
                {
                    _allResolutions.Add(resolutions[i]);
                }
            }
            _allResolutions.Sort((p, q) => p.width * 1000 + p.height - q.width * 1000 - q.height);
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
            if (PlayerPrefs.HasKey(ScreenWidthTag))
            {
                _curResolution.width = PlayerPrefs.GetInt(ScreenWidthTag);
                _curResolution.height = PlayerPrefs.GetInt(ScreenHeightTag);
                _curResolutionIndex = IndexOfResolutions(_curResolution, _allResolutions);
                if (_curResolutionIndex >= 0)
                {
                    _curResolution = _allResolutions[_curResolutionIndex];
                    return;
                }
            }
            //设置默认分辨率
            _curResolution.width = Screen.width;
            _curResolution.height = Screen.height;
            _curResolutionIndex = IndexOfResolutions(_curResolution, _allResolutions);
            if (_curResolutionIndex >= 0)
            {
                _curResolution = _allResolutions[_curResolutionIndex];
                return;
            }
            //若列表中没有，则添加
            _allResolutions.Add(_curResolution);
            _allResolutions.Sort((p, q) => p.width * 1000 + p.height - q.width * 1000 - q.height);
            _curResolutionIndex = _allResolutions.IndexOf(_curResolution);
        }

        public void Save()
        {
            bool needSave = false;
            if (_selectFullScreen != _fullScreen)
            {
                needSave = true;
                _fullScreen = _selectFullScreen;
                PlayerPrefs.SetInt(FullScreenTag, _fullScreen ? 1 : 0);
            }
            if (_selectIndex != _curResolutionIndex)
            {
                needSave = true;
                _curResolutionIndex = _selectIndex;
                _curResolution = _allResolutions[_curResolutionIndex];
                PlayerPrefs.SetInt(ScreenWidthTag, _curResolution.width);
                PlayerPrefs.SetInt(ScreenHeightTag, _curResolution.height);
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

        public void Init()
        {
            SetResolution(_curResolution, _fullScreen);
        }

        public void SetFullScreen(bool value)
        {
            _selectFullScreen = value;
//            SetResolution(_curResolution, value);
        }

        public void SetResolution(Resolution resolution, bool fullScreen)
        {
            _curResolution = resolution;
            _fullScreen = fullScreen;
            Screen.SetResolution(resolution.width, resolution.height, fullScreen);
            Screen.fullScreen = fullScreen;
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

        public void SetResolution(int index)
        {
            if (index >= _allResolutions.Count)
            {
                LogHelper.Error("resolutionIndex > _allResolutions.Count");
                return;
            }
            _selectIndex = index;
//            _curResolutionIndex = index;
//            SetResolution(_allResolutions[index], _fullScreen);
        }
    }
}