using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("BGM")]
    [SerializeField] AudioSource bgmSource;

    [Header("SFX Pool")]
    List<AudioSource> sfxSources = new List<AudioSource>();
    public int poolSize = 10;
    [SerializeField] private AudioClip ObjectClip;
    [SerializeField] private AudioClip moneyClip;
    [SerializeField] private AudioClip createMoneyClip;
    [SerializeField] private AudioClip unLockClip;
    
    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource sfx = gameObject.AddComponent<AudioSource>();
            sfx.playOnAwake = false;
            sfxSources.Add(sfx);
        }
    }
    public void PlaySFX(Define.SFXType sfxType)
    {
        foreach (AudioSource auidoSource in sfxSources)
        {
            if (auidoSource.isPlaying == false)
            {
                SFX(sfxType, auidoSource);
                return;
            }
        }
        // 없으면 첫 번째 강제로 사용
        SFX(sfxType, sfxSources[0]);
    }
    private void SFX(Define.SFXType sfxType, AudioSource auidoSource)
    {
        switch (sfxType)
        {
            case Define.SFXType.Object:
                auidoSource.PlayOneShot(ObjectClip);
                break;
            case Define.SFXType.Money:
                auidoSource.PlayOneShot(moneyClip);
                break;
            case Define.SFXType.CreateMoney:
                auidoSource.PlayOneShot(createMoneyClip);
                break;
            case Define.SFXType.UnLock:
                auidoSource.PlayOneShot(unLockClip);
                break;
        }
    }
}