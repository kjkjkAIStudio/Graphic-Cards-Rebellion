using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    public GameObjectPool sePool;
    public AudioSource audio_bgm;
    public Animator ani_bgm;

    public static SoundMgr Instance { get; private set; }

    public void RefreshBGMVol()
    {
        if (Instance != null)
        {
            ani_bgm.SetFloat("vol", Global.savePack.bgmVol);
            PlayBGM(nextBgm);
        }
    }

    public void PlaySE(AudioClip clip)
    {
        AudioSource temp = ((SE_Recycle)sePool.Get()).se;
        temp.clip = clip;
        temp.volume = Global.savePack.seVol;
        temp.Play();
    }

    public void PlayBGM(AudioClip clip)
    {
        nextBgm = clip;
        ani_bgm.SetTrigger("change");
    }

    public void ChangeEnd()
    {
        audio_bgm.clip = nextBgm;
        audio_bgm.Play();
    }

    AudioClip nextBgm;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ani_bgm.Play("bgm_hide", 0, 1.0f);
        RefreshBGMVol();
    }
}
