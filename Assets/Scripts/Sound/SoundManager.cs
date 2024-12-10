using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]

public class Sound
{
    public string name;                     // ������ �̸�
    public AudioClip clip;                  // ���� Ŭ��

    [Range(0f, 1f)]                         // �ν����Ϳ��� ���� ����
    public float volume = 1.0f;             // ���� ����

    [Range(0.1f, 3f)]
    public float pitch = 1.0f;              // ���� ��ġ
    public bool loop;                       // �ݺ� ��� ����
    public AudioMixerGroup mixerGroup;      // ����� �ͼ� �׷�

    [HideInInspector]                       // �ν����� â���� �Ⱥ��̰� ������.
    public AudioSource source;              // ����� �ҽ�
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;        // �̱��� �ν��Ͻ� ȭ ��Ų��.

    public List<Sound> sounds = new List<Sound>();      // ���� ����Ʈ(�迭���� �쿬�� �ڷᱸ��)
    public AudioMixer audioMixer;                       // ����� �ͼ� ����

    private float savedMusicTime = 0f;

    public bool isSoundOff = false;  //���� �������� ���带 ���µ� ����� bool��

    void Awake()
    {
        if (instance == null)
        {
            instance = this;                    // �̱��� ���� ����
            DontDestroyOnLoad(gameObject);      // Scene�� ����Ǿ �� ������Ʈ�� �ı� X
        }
        else
        {
            Destroy(gameObject);                // �̹� �̱��� ������Ʈ�� ������� �ı��Ѵ�.
        }

        foreach (Sound sound in sounds)         // ����Ʈ �ȿ� �ִ� ������� �ʱ�ȭ�Ѵ�.
        {
            sound.source = gameObject.AddComponent<AudioSource>();      // �ҽ� �ϳ��� 1���� ������Ʈ�� �����ش�.
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sound.mixerGroup;      // ����� �ͼ� �׷� ����
        }
    }

    public void PlaySound(string name)                                      // �μ� Name �޾Ƽ�
    {
        Sound soundToPlay = sounds.Find(sound => sound.name == name);       // List �ȿ� �ִ� name �� �������� �˻� �� soundToPlay�� ����

        if (soundToPlay != null && !isSoundOff)  //���ξ����� ���弳���� �ؼ� ������ ���� ���ǹ�
        {
            soundToPlay.source.Play();
        }
        else
        {
            Debug.Log("���� : " + name + " �����ϴ�.");
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