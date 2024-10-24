using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �۹��� ������ ����ִ� Ŭ����
public class Crop : MonoBehaviour
{
    public enum SeedPlantedState
    {
        Yes,
        No
    }

    public enum CropState
    {
        Empty,
        SeedPlanted,
        NeedsWater,
        ReadyToHarvest,
        Watered  // ���� �� ���� �߰�
    }

    public CropState cropState;
    public SeedPlantedState seedPlantedState;

    public Vector2 seedPosition;

    public Grid grid;

    public GameObject[] growthStages; // ���� �ܰ躰�� �Ҵ�� ���ҽ� ������Ʈ�� (�� 5��)
    private float[] growthTimes;  // �� ���� �ܰ迡 �ʿ��� �ð�
    private int currentStage = 0;  // ���� �۹��� ���� �ܰ�
    private float growthStartTime; // ������ ���۵� �ð�

    private AIStateManager aiStateManager;

    private void Awake()
    {
        aiStateManager = FindObjectOfType<AIStateManager>();

        if (aiStateManager != null)
        {
            aiStateManager.AddSeed(this);
        }
    }

    private void Start()
    {
        grid = FindObjectOfType<Grid>();

        if (grid == null)
        {
            Debug.LogError("Grid �ý����� ã�� �� �����ϴ�!");
            return;
        }

        // ���� ������Ʈ�� ���� ��ǥ���� �׸��� ��ǥ�� ��ȯ
        SetFieldPosition(transform.position);
    }

    // �׸��� ��ġ ���� �޼���
    public void SetFieldPosition(Vector3 worldPosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(worldPosition);

        // Vector3Int���� x�� y ���� �����Ͽ� Vector2�� ��ȯ
        seedPosition = new Vector2(gridPosition.x, gridPosition.y);
        Debug.Log($"�� ��ġ ������: {seedPosition}");
    }

    public bool IsSeedPlanted()
    {
        return seedPlantedState == SeedPlantedState.Yes;
    }

    public bool IsSeedPlantedNo()
    {
        return !IsSeedPlanted();
    }

    public bool NeedsWater()
    {
        return cropState == CropState.NeedsWater;
    }

    public bool IsReadyToHarvest()
    {
        return cropState == CropState.ReadyToHarvest;
    }

    public void CheckSeedPlanted()
    {
        if (seedPlantedState == SeedPlantedState.Yes)
        {
            // ������ �ɾ��� ������ �� ���� �� �� �ִ� ���·� ����
            if (currentStage == 0)
            {
                cropState = CropState.NeedsWater;
                Debug.Log("���� �ʿ��մϴ�");
            }
            else
            {
                cropState = CropState.ReadyToHarvest; // ���� �ܰ迡 ���� ���� ���� (��: 1�ܰ� �̻��� ���)
                Debug.Log("�۹��� �غ�Ǿ����ϴ�.");
            }
        }
    }

    // ���� �ִ� �޼��� �߰�
    public void WaterCrop()
    {
        // ������ �ɾ��� �ְ�, ���� �ܰ谡 0�� �� ���� �� �� �ֵ��� ����
        if (IsSeedPlanted() && currentStage == 0)
        {
            cropState = CropState.Watered;  // ���� �� ���·� ����
            Debug.Log("���� ����ϴ�");
        }
        else if (IsSeedPlanted() && cropState == CropState.Watered)
        {
            Debug.Log("�۹��� �̹� ���� �־������ϴ�.");
        }
        else if (IsSeedPlantedNo() && cropState == CropState.Empty)
        {
            Debug.Log("���� �� �� �����ϴ�: ������ �ɾ��� ���� �ʰų� ���� �̹� �ѷ��� ���� ��");
        }
        else
        {
            Debug.Log("���� ���� �� �� �����ϴ�: �۹��� ���� �ܰ� 0�� �ƴմϴ�.");
        }
    }


    // ���� �ɱ� �޼���
    public void PlantSeed()
    {
        // cropState�� Empty�̰� seedPlantedState�� No�� ���� ������ ���� �� �ֵ��� ����
        if (cropState == CropState.Empty && seedPlantedState == SeedPlantedState.No)
        {
            cropState = CropState.SeedPlanted;
            seedPlantedState = SeedPlantedState.Yes;
            CheckSeedPlanted(); // ���� ���� �� ���� �ʿ��� ���·� ����
            Debug.Log("������ �ɾ����ϴ�.");
        }
        else
        {
            Debug.Log("������ ���� �� �����ϴ�: �̹� �ɾ��� �ֽ��ϴ�.");
        }
    }

    // �۹� ��Ȯ �޼��� �߰�
    public void Harvest()
    {
        if (IsReadyToHarvest())
        {
            cropState = CropState.Empty; // ��Ȯ �� ���¸� �����
            seedPlantedState = SeedPlantedState.No; // ���� ���� �ʱ�ȭ
            Debug.Log("�۹��� ��Ȯ�߽��ϴ�.");
        }
        else
        {
            Debug.Log("�۹��� ��Ȯ�� �� �����ϴ�: ��Ȯ�� �غ� �Ǿ� ���� �ʽ��ϴ�.");
        }
    }

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
