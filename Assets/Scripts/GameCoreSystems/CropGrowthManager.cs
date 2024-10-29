using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropGrowthManager : MonoBehaviour
{
    public static CropGrowthManager Instance;

    private List<Crop> crops = new List<Crop>();    // �ɾ��� �۹����� ������ ����Ʈ

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

    // ����Ʈ�� �۹��� �߰��ϴ� �޼���
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

    // 1�ʸ��� ��� �۹��� ���� ������ üũ�ϴ� ��ƾ
    IEnumerator StartGrowthCheck()
    {
        while (true)    // �׽� üũ�ؾ� �ϹǷ�
        {
            yield return new WaitForSeconds(1f); // 1�� �������� üũ

            float currentTime = Time.time;
            foreach (var crop in crops)
            {
                if (crop == null)
                {
                    continue;
                }
                else
                {
                    crop.CheckGrowth(currentTime); // ���� ���� üũ
                }
            }
        }
    }
}
