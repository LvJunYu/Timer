using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface YIMPlayListener
{
    void OnPlayStop();
}

public class YIMAudioPlayer : MonoBehaviour
{
    //播放回调
    public YIMPlayListener playListener;

    private AudioSource audioSource;
    private AudioClip lastLoadedAudioClip;

    private static YIMAudioPlayer _instance;

    public static YIMAudioPlayer Instance
    {
        get
        {
            if (_instance == null)
            {
                var youmeObj = new GameObject("YouMeIMAudioPlayer");
                GameObject.DontDestroyOnLoad(youmeObj);
                youmeObj.hideFlags = HideFlags.DontSave;
                _instance = youmeObj.AddComponent<YIMAudioPlayer>();
                _instance.audioSource = youmeObj.AddComponent<AudioSource>();
            }
            return _instance;
        }
    }

    public void PlayAudioFile(string audioFilPath)
    {
        StartCoroutine(LoadAndPlayAudio(audioFilPath));
    }

    Coroutine playFinishEnmerator = null;

    private IEnumerator LoadAndPlayAudio(string audioPath)
    {
        if (string.IsNullOrEmpty(audioPath))
        {
            yield return null;
            //call back  finish
        }
        else
        {
            if (!audioPath.Contains("://"))
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                audioPath = "file:///" + audioPath;
#else
                audioPath = "file://" + audioPath;
#endif
            }
            WWW audioLoader = new WWW(audioPath);
            while (!audioLoader.isDone)
            {
                yield return null;
            }
            lastLoadedAudioClip = audioLoader.GetAudioClip(false, false);
            if (lastLoadedAudioClip != null)
            {
                PlayLastLoadedAudio();
            }
        }
        yield return 0;
    }

    private IEnumerator NotifyPlayFinishAfter(float delay)
    {
        if (playListener != null)
        {
            yield return new WaitForSeconds(1);
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            playListener.OnPlayStop();
        }
        else
        {
            yield return null;
        }
        playFinishEnmerator = null;
    }

    private void PlayLastLoadedAudio()
    {
        if (lastLoadedAudioClip != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.clip = lastLoadedAudioClip;
            audioSource.Play();
            if (playFinishEnmerator != null)
            {
                StopCoroutine(playFinishEnmerator);
            }
            playFinishEnmerator = StartCoroutine(NotifyPlayFinishAfter(lastLoadedAudioClip.length + 0.1f));
        }
    }

    public void StopPlay()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            if (playListener != null)
            {
                playListener.OnPlayStop();
            }
        }
    }
}