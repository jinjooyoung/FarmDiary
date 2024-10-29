using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropGrowthManager : MonoBehaviour
{
    public static CropGrowthManager Instance;

    private List<Crop> crops = new List<Crop>();    // 심어진 작물들을 저장할 리스트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(StartGrowthCheck());
    }

    // 리스트에 작물을 추가하는 메서드
    public void RegisterCrop(Crop crop)
    {
        for (int i = 0; i < crops.Count; i++)
        {
            if (crops[i] == null)
            {
                crops[i] = crop;
                return;
            }
        }

        crops.Add(crop);
    }

    // 1초마다 모든 작물의 성장 정도를 체크하는 루틴
    IEnumerator StartGrowthCheck()
    {
        while (true)    // 항시 체크해야 하므로
        {
            yield return new WaitForSeconds(1f); // 1초 간격으로 체크

            float currentTime = Time.time;
            foreach (var crop in crops)
            {
                if (crop == null)
                {
                    continue;
                }
                else
                {
                    crop.CheckGrowth(currentTime); // 성장 여부 체크
                }
            }
        }
    }
}
