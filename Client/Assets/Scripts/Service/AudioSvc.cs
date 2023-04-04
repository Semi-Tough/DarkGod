/****************************************************
    文件：AudioService.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/23 12:56:8
    功能：音频服务
*****************************************************/

using PETool.PELogger;
using UnityEngine;
public class AudioSvc : MonoBehaviour {
	public static AudioSvc Instance { private set; get; }
	public AudioSource bgAudio;
	public AudioSource uiAudio;

	public void InitService() {
		Instance = this;
		PELogger.Log("音频服务加载完成");
	}

	public void PlayBgMusic(string music, bool isLoop = true) {
		AudioClip audioClip = ResSvc.Instance.LoadAudioClip("ResAudios/" + music, true);
		if(bgAudio.clip == null || bgAudio.clip.name != music) {
			bgAudio.clip = audioClip;
			bgAudio.loop = isLoop;
			bgAudio.Play();
		}
	}
	public void StopBgMusic() {
		bgAudio.Stop();
	}

	public void Play2DAudio(string clip, float volume = 0.5f) {
		AudioClip audioClip = ResSvc.Instance.LoadAudioClip("ResAudios/" + clip, true);
		uiAudio.clip = audioClip;
		uiAudio.volume = volume;
		uiAudio.Play();
	}
	public void Play3DAudio(string clip, AudioSource source) {
		AudioClip audioClip = ResSvc.Instance.LoadAudioClip("ResAudios/" + clip, true);
		source.clip = audioClip;
		source.Play();
	}
}