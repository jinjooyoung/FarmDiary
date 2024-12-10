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
        // ������ �����̴��� ���� ���� �ɶ� �����ʸ� ���ؼ� �ϼ��� ���� �����Ѵ�.
        musicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    void Start()
    {
        musicMasterSlider.value = PlayerPrefs.GetFloat("Master", 1f);       // ������ ���� �����´�
    }

    public void SetMasterVolume(float volume)
    {
        // ���� �ּҰ��� 0.0001�� ����
        float adjustedVolume = Mathf.Max(volume, 0.0001f);

        // ������ ���� �����̴� ���� Mixer�� �ݿ�
        audioMixer.SetFloat("Master", Mathf.Log10(adjustedVolume) * 20);

        // �����̴� ���� ���ÿ� ����
        PlayerPrefs.SetFloat("Master", volume);
    }
}