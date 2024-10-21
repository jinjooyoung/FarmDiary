using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropGrowthManager : MonoBehaviour
{
    private List<Crop> crops = new List<Crop>();    // �ɾ��� �۹����� ������ ����Ʈ

    // ����Ʈ�� �۹��� �߰��ϴ� �޼���
    public void RegisterCrop(Crop crop)
    {
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
                crop.CheckGrowth(currentTime); // ���� ���� üũ
            }
        }
    }
}

// �۹��� ������ ����ִ� Ŭ����
public class Crop : MonoBehaviour
{
    private int growthStage = 0;
    private float[] growthTimes; // �� �ܰ踶�� ���忡 �ɸ��� �ð� (�� ����)
    private float nextGrowthTime = 0f;

    public void Initialize(float[] growthDurations)
    {
        this.growthTimes = growthDurations;
        StartGrowing(); // �۹� �ɱ� ����
    }

    // �۹��� �ɾ��� �� ù �ܰ迡 ����
    public void StartGrowing()
    {
        nextGrowthTime = Time.time + growthTimes[growthStage]; // ù ��° �ܰ�� ����
    }

    // ���� ���¸� Ȯ���ϰ� �ܰ� ����
    public void CheckGrowth(float currentTime)
    {
        if (currentTime >= nextGrowthTime && growthStage < growthTimes.Length - 1) // ���� �ִ� �ܰ迡 �������� ���� ���
        {
            growthStage++;
            UpdateVisual(); // ���ҽ� ������Ʈ (SetActive ��)

            // ���� ���� �ܰ���� �ð� ����
            nextGrowthTime = currentTime + growthTimes[growthStage];
        }
    }

    // �� ���� �ܰ迡 �°� ���ҽ� ��ȭ ó��
    private void UpdateVisual()
    {
        Debug.Log($"�۹��� �����Ͽ� ���� {growthStage} �ܰ��Դϴ�.");
        // SetActive(true/false) �ڵ� �ۼ� �ؾ���
    }
}
