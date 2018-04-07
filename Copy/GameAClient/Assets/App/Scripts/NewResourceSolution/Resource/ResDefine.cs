using System.IO;
using UnityEngine;

namespace NewResourceSolution
{
    public static class ResDefine
    {
        public static string ServerVersionConfigFileName = "ServerVersionConfig";
        public static string CHResManifestFileName = "Manifest";
        public static string UnityManifestBundleName = "raw";
        public static char LocaleResBundleNameFirstChar = '@';
        public static char SplashCharInAssetBundleName = '/';
        public static char ReplaceSplashCharInAssetBundleName = '@';
        public static int GetServerVersionConfigTimeout = 5000;
        public static int GetServerManifestTimeout = 5000;

        public static int ResGroupInPackage = 0;
        public static int ResGroupNecessary = 1;

        public static long KiloByte = 1024;
    }

    public static class ResPath
    {
        public static string Assets = "Assets";
        public static string EditorRawRes = "JoyGameArt";
        public static string LocaleResRoot = "Locale";
        public static string Prefabs = "Prefabs";
        public static string Behaviors = "Behaviors";
        public static string UIPrefabs = "UI";
        public static string Sprites = "Sprites";
        public static string JsonTables = "Tables";
        public static string Textures = "Textures";
        public static string Materials = "Materials";
        public static string Animations = "Animations";
        public static string Audio = "Audio";
        public static string Fonts = "Fonts";
        public static string MeshDatas = "MeshDatas";
        public static string SpineDatas = "SpineDatas";
        public static string Shaders = "Shaders";
        public static string Tables = "Tables";
        public static string Particles = "Particles";
        public static string Models = "Models";
        public static string StreamingAssets = "StreamingAssets";
        public static string PersistentBundleRoot = "AssetBundles";
        public static string TempCache = "_temp";

        public static string PrefabExtension = ".prefab";
        public static string JsonExtension = ".json";
        public static string MaterialExtension = ".mat";
        public static string AudioExtension = ".ogg";
        public static string FontExtension = ".ttf";
        public static string MeshExtension = ".fbx";
        public static string ShaderExtension = ".shader";
        public static string TableExtension = ".json";
        public static string TextureExtension = ".png";
        public static string SpriteExtension = ".png";
        public static string AssetBundleOutputPath = "../../AssetBundles";
        public static string PlatformFolderiOS = "iOS";
        public static string PlatformFolderAndroid = "Android";
        public static string PlatformFolderWin = "Win";
        public static string PlatformFolderOSX = "OSX";

        public static string GetPlatformFolder()
        {
            string folder;
            switch (Application.platform)
            {
                case RuntimePlatform.OSXPlayer:
                    folder = PlatformFolderOSX;
                    break;
                case RuntimePlatform.WindowsPlayer:
                    folder = PlatformFolderWin;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    folder = PlatformFolderiOS;
                    break;
                case RuntimePlatform.Android:
                    folder = PlatformFolderAndroid;
                    break;
                case RuntimePlatform.WindowsEditor:
                    folder = PlatformFolderWin;
                    break;
                case RuntimePlatform.OSXEditor:
                    folder = PlatformFolderOSX;
                    break;
                default:
                    folder = PlatformFolderOSX;
                    break;
            }
            return folder;
        }

        private static string _persistentDataPath;
        public static string PersistentDataPath
        {
            get
            {
                if (_persistentDataPath == null)
                {
                    if (!Application.isEditor && !Application.isMobilePlatform)
                    {
                        _persistentDataPath = Application.dataPath + "/PersistentDataPath";
                    }
                    else
                    {
                        _persistentDataPath = Application.persistentDataPath;
                    }
                }
                return _persistentDataPath;
            }
        }

        private static string _runtimeStreamingAssetsPath;
        public static string RuntimeStreamingAssetsPath
        {
            get
            {
                if (_runtimeStreamingAssetsPath == null)
                {
                    if (Application.isEditor)
                    {
                        _runtimeStreamingAssetsPath = string.Format(StringFormat.FourLevelPath,
                            Application.dataPath, AssetBundleOutputPath, GetPlatformFolder(), StreamingAssets);
                    }
                    else
                    {
                        _runtimeStreamingAssetsPath = Application.streamingAssetsPath;
                    }
                }
                return _runtimeStreamingAssetsPath;
            }
        }

    }

    public static class StringFormat
    {
        public static string TwoLevelPath = "{0}/{1}";
        public static string TwoLevelPathWithExtention = "{0}/{1}.{2}";
        public static string ThreeLevelPath = "{0}/{1}/{2}";
        public static string ThreeLevelPathWithExtention = "{0}/{1}/{2}.{3}";
        public static string FourLevelPath = "{0}/{1}/{2}/{3}";
        public static string FourLevelPathWithExtention = "{0}/{1}/{2}/{3}.{4}";
        public static string FiveLevelPath = "{0}/{1}/{2}/{3}/{4}";
        public static string FiveLevelPathWithExtention = "{0}/{1}/{2}/{3}/{4}.{5}";
        public static string SixLevelPath = "{0}/{1}/{2}/{3}/{4}/{5}";
        public static string SixLevelPathWithExtention = "{0}/{1}/{2}/{3}/{4}/{5}.{6}";
    }

    /// <summary>
    /// 资源类型
    /// </summary>
    public enum EResType
    {
        None,
        Txt,
        Json,
        Table,
        Texture,
        Sprite,
        Font,
        SpineData,
        Shader,
        Material,
        AudioClip,
        MeshData,
        Animation,
        ParticlePrefab,
        UIPrefab,
        ModelPrefab,
        Behavior,
        Max
    }

    /// <summary>
    /// 资源压缩状态
    /// </summary>
    public enum EAssetBundleCompressType
    {
        NoCompress = 0,
        LZMA = 1,
    }

    /// <summary>
    /// 文件所在位置
    /// </summary>
    public enum EFileLocation
    {
        Server = 0,
        StreamingAsset = 1,
        Persistent = 2,
        TemporaryCache = 3,
    }

    public enum EFileIntegrity
    {
        NotExist,
        Md5Dismatch,
        Integral,
    }
}