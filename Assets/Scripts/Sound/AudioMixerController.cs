using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicMasterSlider;

    private void Awake()
    {
        // 마스터 슬라이더의 값이 변경 될때 리스너를 통해서 하수에 값을 전달한다.
        musicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    void Start()
    {
        musicMasterSlider.value = PlayerPrefs.GetFloat("Master", 1f);       // 저장한 값을 가져온다
    }

    public void SetMasterVolume(float volume)
    {
        // 볼륨 최소값을 0.0001로 제한
        float adjustedVolume = Mathf.Max(volume, 0.0001f);

        // 마스터 볼륨 슬라이더 값을 Mixer에 반영
        audioMixer.SetFloat("Master", Mathf.Log10(adjustedVolume) * 20);

        // 슬라이더 값을 로컬에 저장
        PlayerPrefs.SetFloat("Master", volume);
    }
}