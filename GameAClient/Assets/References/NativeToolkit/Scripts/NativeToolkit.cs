/********************************************************************
** Filename : NativeToolkit
** Author : Dong
** Date : 2015/11/26 星期四 上午 11:12:53
** Summary : NativeToolkit
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;

#if UNITY_IOS
using UnityEngine.iOS;
using System.Runtime.InteropServices;
#endif

namespace SoyEngine
{
    public enum ETextureType
    {
        None,
        Png,
        Jpg,
        Max
    }

    public enum ESaveStatus
    {
        NotSaved,
        Saved,
        Denied,
        Timeout
    }

    public class NativeToolkit : MonoBehaviour
    {
        private static NativeToolkit _instance;
        private static GameObject _go;

        public static NativeToolkit Instance
        {
            get
            {
                if (_instance == null)
                {
                    _go = new GameObject {name = "NativeToolkit"};
                    _instance = _go.AddComponent<NativeToolkit>();

#if UNITY_ANDROID

                    if (Application.platform == RuntimePlatform.Android)
                    {
                        _androidObj = new AndroidJavaClass("com.secondfury.nativetoolkit.Main");
                    }

#elif UNITY_WINRT

				NativeToolkitWP8.Main.OnWP8DialogPress += _instance.OnDialogPress;
				NativeToolkitWP8.Main.OnWP8CameraFinish += _instance.OnCameraFinished;
				NativeToolkitWP8.Main.OnWP8PickImageFinish += _instance.OnPickImage;

#endif
                }
                return _instance;
            }
        }

#if UNITY_IOS

        [DllImport("__Internal")]
        private static extern int saveToGallery(string path);

        [DllImport("__Internal")]
        private static extern void pickImage();

        [DllImport("__Internal")]
        private static extern void openCamera();

        [DllImport("__Internal")]
        private static extern void pickContact();

        [DllImport("__Internal")]
        private static extern string getLocale();

        [DllImport("__Internal")]
        private static extern void sendEmail(string to, string cc, string bcc, string subject, string body,
            string imagePath);

        [DllImport("__Internal")]
        private static extern void scheduleLocalNotification(string id, string title, string message, int delayInMinutes,
            string sound);

        [DllImport("__Internal")]
        private static extern void clearLocalNotification(string id);

        [DllImport("__Internal")]
        private static extern void clearAllLocalNotifications();

        [DllImport("__Internal")]
        private static extern bool wasLaunchedFromNotification();

        [DllImport("__Internal")]
        private static extern void rateApp(string title, string message, string positiveBtnText, string neutralBtnText,
            string negativeBtnText, string appleId);

        [DllImport("__Internal")]
        private static extern void showConfirm(string title, string message, string positiveBtnText,
            string negativeBtnText);

        [DllImport("__Internal")]
        private static extern void showAlert(string title, string message, string confirmBtnText);

#elif UNITY_ANDROID

        private static AndroidJavaClass _androidObj;

#endif
        public static Action<Texture2D> OnScreenshotTaken;
        public static Action<string> OnScreenshotSaved;
        public static Action<ESaveStatus> OnImageSaved;
        public static Action<Texture2D, string> OnImagePicked;
        public static Action<bool> OnDialogComplete;
        public static Action<string> OnRateComplete;
        public static Action<Texture2D, string> OnCameraShotComplete;
        public static Action<string, string, string> OnContactPicked;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
        }

        public static Texture2D LoadImageFromFile(string path)
        {
            if (path == "Cancelled") return null;
            byte[] bytes;
            var texture = new Texture2D(128, 128, TextureFormat.RGB24, false);
#if UNITY_WINRT

		bytes = UnityEngine.Windows.File.ReadAllBytes(path);
		texture.LoadImage(bytes);

#else

            bytes = File.ReadAllBytes(path);
            texture.LoadImage(bytes);

#endif
            return texture;
        }

        #region Gallery

        public void PickImage()
        {
            Instance.Awake();
#if UNITY_IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                pickImage();
            }
#elif UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
				_androidObj.CallStatic("pickImageFromGallery");
            }
#elif UNITY_WINRT
            Main.PickImage(OnPickImage);
#endif
        }

        public void OnPickImage(string path)
        {
            StartCoroutine(LoadPickImage(path));
        }

        private IEnumerator LoadPickImage(string path)
        {
#if UNITY_WINRT

		yield return new WaitForEndOfFrame ();

		#endif

            Texture2D texture = LoadImageFromFile(path);
            if (OnImagePicked != null)
            {
                OnImagePicked(texture, path);
            }
            yield return 0;
        }

        public void SaveImage(Texture2D texture, string fileName, ETextureType eTextureType)
        {
            Debug.Log("Save image to gallery " + fileName);

            Instance.Awake();

            byte[] bytes = null;
            string fileExt = "";
            switch (eTextureType)
            {
                case ETextureType.Jpg:
                    bytes = texture.EncodeToJPG();
                    fileExt = ".jpg";
                    break;
                case ETextureType.Png:
                    bytes = texture.EncodeToPNG();
                    fileExt = ".png";
                    break;
            }
            string path = Application.persistentDataPath + "/" + fileName + fileExt;
            StartCoroutine(Save(bytes, fileName, path));
        }

        private IEnumerator Save(byte[] bytes, string fileName, string path)
        {
            int count = 0;
            var saveStatus = ESaveStatus.NotSaved;

#if UNITY_IOS

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                File.WriteAllBytes(path, bytes);
                while (saveStatus == ESaveStatus.NotSaved)
                {
                    count++;
                    if (count > 3)
                    {
                        saveStatus = ESaveStatus.Timeout;
                    }
                    else
                    {
                        saveStatus = (ESaveStatus) saveToGallery(path);
                    }
                    yield return new WaitForSeconds(0.5f);
                }
                Device.SetNoBackupFlag(path);
            }

#elif UNITY_ANDROID

            if (Application.platform == RuntimePlatform.Android)
            {
                File.WriteAllBytes(path, bytes);
                while (saveStatus == ESaveStatus.NotSaved)
                {
                    count++;
                    if (count > 3)
                    {
                        saveStatus = ESaveStatus.Timeout;
                    }
                    else
                    {
                        saveStatus = (ESaveStatus) NativeToolkit._androidObj.CallStatic<int>("addImageToGallery", path);
                    }
                    yield return new WaitForSeconds(0.5f);
                }
            }

#elif UNITY_WINRT

            if (Application.platform == RuntimePlatform.WP8Player)
            {
                Main.SaveImage(bytes, fileName);
                saveStatus = ESaveStatus.Saved;
                yield return null;
            }

#else

            Debug.Log("Native Toolkit: Save file only available in iOS/Android/WP8 modes");
            saveStatus = ESaveStatus.Saved;
            yield return null;

#endif
            if (OnImageSaved != null)
            {
                OnImageSaved(saveStatus);
            }
        }

        #endregion

        #region Camera

        public static void TakeCameraShot()
        {
            Instance.Awake();
#if UNITY_IOS

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                openCamera();
            }

#elif UNITY_ANDROID

            if (Application.platform == RuntimePlatform.Android)
            {
				_androidObj.CallStatic("takeCameraShot");
            }

#elif UNITY_WINRT

            Main.OpenCamera(Instance.OnCameraFinished);

#endif
        }

        public void OnCameraFinished(string path)
        {
            StartCoroutine(LoadCameraImage(path));
        }

        private IEnumerator LoadCameraImage(string path)
        {
#if UNITY_WINRT

		yield return new WaitForEndOfFrame ();

#endif
            Texture2D texture = LoadImageFromFile(path);
            if (OnCameraShotComplete != null)
            {
                OnCameraShotComplete(texture, path);
            }
            yield return 0;
        }

        #endregion

        #region Contacts

        public static void PickContact()
        {
            Instance.Awake();
#if UNITY_IOS

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                pickContact();
            }

#elif UNITY_ANDROID

            if (Application.platform == RuntimePlatform.Android)
            {
				_androidObj.CallStatic("pickContact");
            }

#elif UNITY_WINRT

            Debug.Log("WP8 contact support coming soon...");

#endif
        }

        public void OnPickContactFinished(string data)
        {
            var details = Json.Deserialize(data) as Dictionary<string, object>;
            if (details == null)
            {
                return;
            }
            string name = "";
            string number = "";
            string email = "";
            if (details.ContainsKey("name"))
            {
                name = details["name"].ToString();
            }
            if (details.ContainsKey("number"))
            {
                number = details["number"].ToString();
            }
            if (details.ContainsKey("email"))
            {
                email = details["email"].ToString();
            }

            if (OnContactPicked != null)
            {
                OnContactPicked(name, number, email);
            }
        }

        #endregion

        #region Confirm Dialog/Alert

        public static void ShowConfirm(string title, string message, Action<bool> callback = null,
            string positiveBtnText = "Ok", string negativeBtnText = "Cancel")
        {
            Instance.Awake();

            OnDialogComplete = callback;

#if UNITY_IOS

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                showConfirm(title, message, positiveBtnText, negativeBtnText);
            }

#elif UNITY_ANDROID

            if (Application.platform == RuntimePlatform.Android)
            {
				_androidObj.CallStatic("showConfirm", new object[] {title, message, positiveBtnText, negativeBtnText});
            }

#elif UNITY_WINRT

            Main.ShowConfirm(title, message, Instance.OnDialogPress);

#endif
        }

        public static void ShowAlert(string title, string message, Action<bool> callback = null, string btnText = "Ok")
        {
            Instance.Awake();

            OnDialogComplete = callback;

#if UNITY_IOS

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                showAlert(title, message, btnText);
            }

#elif UNITY_ANDROID

            if (Application.platform == RuntimePlatform.Android)
            {
				_androidObj.CallStatic("showAlert", new object[] {title, message, btnText});
            }

#elif UNITY_WINRT

            Main.ShowAlert(title, message, Instance.OnDialogPress);

#endif
        }

        public void OnDialogPress(string result)
        {
            if (OnDialogComplete != null)
            {
                if (result == "Yes")
                {
                    OnDialogComplete(true);
                }
                else if (result == "No")
                {
                    OnDialogComplete(false);
                }
            }
        }

        #endregion

        #region Rate this app

        public static void RateApp(string title = "Rate This App",
            string message = "Please take a moment to rate this App",
            string positiveBtnText = "Rate Now", string neutralBtnText = "Later", string negativeBtnText = "No, Thanks",
            string appleId = "", Action<string> callback = null)
        {
            Instance.Awake();

            OnRateComplete = callback;

#if UNITY_IOS

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (appleId != "")
                {
                    rateApp(title, message, positiveBtnText, neutralBtnText, negativeBtnText, appleId);
                }
            }

#elif UNITY_ANDROID

            if (Application.platform == RuntimePlatform.Android)
            {
				_androidObj.CallStatic("rateThisApp",
                    new object[] {title, message, positiveBtnText, neutralBtnText, negativeBtnText});
            }

#elif UNITY_WINRT

            Main.RateThisApp();

#endif
        }

        public void OnRatePress(string result)
        {
            if (OnRateComplete != null)
            {
                OnRateComplete(result);
            }
        }

        #endregion
    }
}