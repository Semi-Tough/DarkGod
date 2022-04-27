/****************************************************
    文件：AudioService.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/23 12:56:8
    功能：音频服务
*****************************************************/

using PEProtocol;
using UnityEngine;

public class AudioService : MonoBehaviour
{
    public static AudioService instance;
    public AudioSource bgAudio;
    public AudioSource uiAudio;

    public void InitService()
    {
        instance = this;
        PeCommon.Log("音频服务加载完成");
    }

    public void PlayBgMusic(string music, bool isLoop = true)
    {
        AudioClip audioClip = ResourceService.instance.LoadAudioClip("ResAudios/" + music, true);
        if (bgAudio.clip == null || bgAudio.clip.name != music)
        {
            bgAudio.clip = audioClip;
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }

    public void PlayUiAudio(string clip)
    {
        AudioClip audioClip = ResourceService.instance.LoadAudioClip("ResAudios/" + clip, true);
        uiAudio.clip = audioClip;
        uiAudio.Play();
    }
}