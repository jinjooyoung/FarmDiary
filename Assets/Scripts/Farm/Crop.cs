using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �۹��� ������ ����ִ� Ŭ����
public class Crop : MonoBehaviour
{
    public GameObject[] growthStages; // ���� �ܰ躰�� �Ҵ�� ���ҽ� ������Ʈ�� (�� 5��)
    private float[] growthTimes;  // �� ���� �ܰ迡 �ʿ��� �ð�
    private int currentStage = 0;  // ���� �۹��� ���� �ܰ�
    private float growthStartTime; // ������ ���۵� �ð�

    // ���� �ð��� �ʱ�ȭ�ϰ�, ù ���� �ܰ踦 ����
    public void Initialize(float[] cropGrowthTimes)
    {
        growthTimes = cropGrowthTimes;
        growthStartTime = Time.time;  // ������ ���۵� �ð��� ����
        currentStage = 0;             // �ʱ� ���� �ܰ� ����

        UpdateCropVisual();            // �ʱ� ���� ������Ʈ
        UpdateSortingLayer();          // �ʱ� ���� ���̾� ������Ʈ
    }

    // �� �ܰ躰�� ������ üũ�ϰ� ���� ���¸� ������Ʈ
    // CropGrowthManager ��ũ��Ʈ���� ȣ���
    public void CheckGrowth(float currentTime)
    {
        if (currentStage < 4 && currentTime - growthStartTime >= growthTimes[currentStage])
        {
            currentStage++;
            UpdateCropVisual();
            UpdateSortingLayer();
        }
    }

    // �۹��� ���¸� ������Ʈ
    private void UpdateCropVisual()
    {
        // ��� ���� �ܰ踦 �ϴ� ��Ȱ��ȭ
        for (int i = 0; i < growthStages.Length; i++)
        {
            growthStages[i].SetActive(false);
        }

        // ���� �ܰ迡 �ش��ϴ� ������Ʈ�� Ȱ��ȭ
        if (currentStage >= 0 && currentStage < 5)
        {
            growthStages[currentStage].SetActive(true);
        }
    }

    // ���� ���̾� ������Ʈ
    private void UpdateSortingLayer()
    {
        foreach (var stage in growthStages)
        {
            if (stage.activeSelf) // ���� Ȱ��ȭ�� ���� �ܰ踸 ���� ���̾� ����
            {
                SpriteRenderer renderer = stage.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingLayerName = "MiddleGround"; // ���ϴ� ���� ���̾� �̸����� ����
                    renderer.sortingOrder = CalculateSortingOrder(); // ���� ���� ������ ����
                }
            }
        }
    }

    // ��ġ�� ���� ���� ������ ����ϴ� �޼��� (����)
    private int CalculateSortingOrder()
    {
        return (int)(transform.position.y * -10); // Y���� ������ ��ȯ�Ͽ� ����
    }
}
