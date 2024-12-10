using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]

public class Sound
{
    public string name;                     // 사운드의 이름
    public AudioClip clip;                  // 사운드 클립

    [Range(0f, 1f)]                         // 인스펙터에서 범위 설정
    public float volume = 1.0f;             // 사운드 볼륨

    [Range(0.1f, 3f)]
    public float pitch = 1.0f;              // 사운드 피치
    public bool loop;                       // 반복 재생 여부
    public AudioMixerGroup mixerGroup;      // 오디오 믹서 그룹

    [HideInInspector]                       // 인스펙터 창에서 안보이게 가린다.
    public AudioSource source;              // 오디오 소스
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;        // 싱글톤 인스턴스 화 시킨다.

    public List<Sound> sounds = new List<Sound>();      // 사운드 리스트(배열보다 우연한 자료구조)
    public AudioMixer audioMixer;                       // 오디오 믹서 참조

    private float savedMusicTime = 0f;

    public bool isSoundOff = false;  //게임 설정에서 사운드를 끄는데 사용할 bool값

    void Awake()
    {
        if (instance == null)
        {
            instance = this;                    // 싱글톤 패턴 적용
            DontDestroyOnLoad(gameObject);      // Scene이 변경되어도 이 오브젝트는 파괴 X
        }
        else
        {
            Destroy(gameObject);                // 이미 싱글톤 오브젝트가 있을경우 파괴한다.
        }

        foreach (Sound sound in sounds)         // 리스트 안에 있는 사운드들을 초기화한다.
        {
            sound.source = gameObject.AddComponent<AudioSource>();      // 소스 하나당 1개씩 컴포넌트를 더해준다.
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sound.mixerGroup;      // 오디오 믹서 그룹 설정
        }
    }

    public void PlaySound(string name)                                      // 인수 Name 받아서
    {
        Sound soundToPlay = sounds.Find(sound => sound.name == name);       // List 안에 있는 name 이 같은것을 검색 후 soundToPlay에 선언

        if (soundToPlay != null && !isSoundOff)  //메인씬에서 사운드설정을 해서 영향을 받을 조건문
        {
            soundToPlay.source.Play();
        }
        else
        {
            Debug.Log("사운드 : " + name + " 없습니다.");
        }
    }

    public void StopSound(string name)
    {
        Sound stopSoundToPlay = sounds.Find(sound => sound.name == name);

        if (stopSoundToPlay != null)
        {
            stopSoundToPlay.source.Stop();
        }
    }

    public void PauseSound(string name)
    {
        Sound stopSoundToPlay = sounds.Find(sound => sound.name == name);

        if (stopSoundToPlay != null)
        {
            stopSoundToPlay.source.Pause();
        }
    }
}