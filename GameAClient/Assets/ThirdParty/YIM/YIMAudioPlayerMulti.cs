using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YIMAudioPlayerMulti : MonoBehaviour {

	//播放回调
	public YIMPlayListener playListener;
    public static float volume=1.0f;

	private AudioSource audioSource;
    private AudioClip lastLoadedAudioClip;

	// private static YIMAudioPlayer _instance;
    public static YIMAudioPlayerMulti GetNewInstance
    {
        get
        {
            // if (_instance == null)
            // {
                var youmeObj = new GameObject("__YouMeIMAudioPlayer"+Random.Range(1,5000).ToString());
                // GameObject.DontDestroyOnLoad(youmeObj);
                // youmeObj.hideFlags = HideFlags.DontSave;
                YIMAudioPlayerMulti _instance = youmeObj.AddComponent<YIMAudioPlayerMulti>();
                _instance.audioSource = youmeObj.AddComponent<AudioSource>();
            // }
            return _instance;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	 public void PlayAudioFile(string audioFilPath)
    {
        StartCoroutine( LoadAndPlayAudio( audioFilPath ));           
    }

    Coroutine playFinishEnmerator=null;
    private IEnumerator LoadAndPlayAudio(string audioPath)
    {
        if (string.IsNullOrEmpty(audioPath))
        {
            yield return 0;
            //call back  finish
        }
        else
        {
            if (!audioPath.Contains("://"))
            {
#if  UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                audioPath = "file:///" + audioPath;
#else
                audioPath = "file://" + audioPath;
#endif
            }
           
            WWW audioLoader = new WWW(audioPath);
            while (!audioLoader.isDone)
            {
                yield return 0;
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
        yield return new WaitForSeconds(delay);
        if (playListener != null)
        {
			while(audioSource.isPlaying){
                yield return 0;
            }
            playListener.OnPlayStop();
            Destroy(gameObject);
        }else{
            yield return 0;
            Destroy(gameObject);
        }
        playFinishEnmerator = null;
    }

    private void PlayLastLoadedAudio(){
        if (lastLoadedAudioClip != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.clip = lastLoadedAudioClip;
            audioSource.volume = volume;
            audioSource.Play();
            if (playFinishEnmerator != null)
            {
                StopCoroutine(playFinishEnmerator);
            }
            playFinishEnmerator = StartCoroutine(NotifyPlayFinishAfter(lastLoadedAudioClip.length+0.1f));
        }
    }

	public void StopPlay()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Destroy(gameObject);
        }
    }
}
