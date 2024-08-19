using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmAudioMixer;
    [SerializeField] private AudioMixerGroup sfxAudioMixer;

    [Header("# BGM Info")]
    [SerializeField] private AudioClip bgmClip;    
    [SerializeField] private AudioSource bgmSource;
    [SerializeField][Range(0f, 1f)] private float bgmVolume;

    [Header("# SFX Info")]
    [SerializeField] private AudioClip sfxClips;    
    [SerializeField] private AudioSource[] sfxSource;
    [SerializeField][Range(0f, 1f)] private float sfxVolume;
    [SerializeField] private int channels; // 많은 효과음을 내기 위한 채널 시스템
    private int channelIdx;

    protected override void Awake()
    {
        base.Awake();
        InitAudioMixer();
    }

    private void Start()
    {
        //PlayBGM("BGM00001");
        //PlayBGM("STAGEDEFAULT");
    }

    private void InitAudioMixer()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;

        bgmSource = bgmObject.AddComponent<AudioSource>();        
        bgmSource.loop = true; 
        bgmSource.volume = bgmVolume;
        bgmSource.outputAudioMixerGroup = bgmAudioMixer;

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SFXPlayer");
        sfxObject.transform.parent = transform;
        sfxSource = new AudioSource[channels];

        for (int i = 0; i < sfxSource.Length; i++)
        {
            sfxSource[i] = sfxObject.AddComponent<AudioSource>();
            sfxSource[i].playOnAwake = false;
            sfxSource[i].volume = sfxVolume;
            sfxSource[i].bypassListenerEffects = true; // 하이패스에 안 걸리게 함
            sfxSource[i].outputAudioMixerGroup = sfxAudioMixer;
        }
    }

    public async void PlayBGM(string rcode) // BGM 플레이 함수
    {
        bgmClip = await ResourceManager.Instance.GetResource<AudioClip>(rcode, EAddressableType.AUDIO);
        bgmSource.clip = bgmClip;
        bgmSource.Play();
    }

    public async void PlaySFX(string rcode)
    {
        for (int i = 1; i < sfxSource.Length; i++) // 0번은 발자국 소리 채널이기 문에 1 ~ 15의 채널만 순회
        {
            // 예를들어 5번 인덱스를 마지막으로 사용했으면 6 7 8 9 10 1 2 3 4 5 이런식으로 순회하게 하기위한 계산임
            int loopIndex = (i + channelIdx) % sfxSource.Length;

            if (sfxSource[loopIndex].isPlaying) // 해당 채널이 Play 중이라면
            {
                continue;
            }

            //int randomIndex = 0;

            channelIdx = loopIndex;
            sfxSource[loopIndex].clip = await ResourceManager.Instance.GetResource<AudioClip>(rcode, EAddressableType.AUDIO);
            sfxSource[loopIndex].Play();
            break; // 효과음이 빈 채널에서 재생 됐기 때문에 반드시 break로 반복문을 빠져나가야함
        }
    }    

    public void SetAudioMixerVolume(EAudioMixerType type, float volume)
    {
        // 오디오 믹서의 값은 -80 ~ 0까지이기 때문에 0.0001 ~ 1의 Log10 * 20을 한다.
        audioMixer.SetFloat(type.ToString(), Mathf.Log10(volume) * 20);
    }

    public void GetAudioMixerVolume(EAudioMixerType type, out float volume)
    {
        float vol;
        audioMixer.GetFloat(type.ToString(), out vol);
        volume = Mathf.Pow(10, vol / 20);
    }
    public void OnMenuClick()
    {
        PlaySFX("Click");
    }

}
