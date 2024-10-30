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
        NeedsWater,
        ReadyToHarvest,
        Harvested,
        Watered  // ���� �� ���� �߰�
    }

    public CropState cropState;
    public SeedPlantedState seedPlantedState;

    public Vector2 seedPosition;

    public Grid grid;

    public bool isPreview;

    public GameObject[] growthStages; // ���� �ܰ躰�� �Ҵ�� ���ҽ� ������Ʈ�� (�� 5��)
    private float[] growthTimes;  // �� ���� �ܰ迡 �ʿ��� �ð�
    public int currentStage = 0;  // ���� �۹��� ���� �ܰ�
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

    public bool NeedsWater()
    {
        return cropState == CropState.NeedsWater;
    }

    public bool IsWatered()
    {
        return cropState == CropState.Watered;
    }

    public bool IsReadyToHarvest()
    {
        return cropState == CropState.ReadyToHarvest;
    }

    public bool Harvested()
    {
        return cropState == CropState.Harvested;
    }

    public void CheckSeedPlanted()
    {
        if (seedPlantedState == SeedPlantedState.Yes)
        {
            cropState = CropState.NeedsWater; // ������ �ɾ����� ���� �ʿ��� ���·� ����
            Debug.Log("������ �ɾ������ϴ�. ���� �ʿ��մϴ�.");
        }
    }

    // ���� �ɱ� �޼���
    public void PlantSeed()
    {
        if (!isPreview)
        {
            cropState = CropState.NeedsWater; // ���� �� ���� �ʿ��ϰ� ����
            seedPlantedState = SeedPlantedState.Yes;
            currentStage = 0;  // �ʱ� ���� �ܰ� ����
            growthStartTime = 0; // �ʱ�ȭ�Ͽ� ������ ���ߵ��� ����

            Debug.Log("������ �ɾ����ϴ�. ���� �ʿ��մϴ�.");

            // AI���� ���ο� ������ Ȯ���ϵ��� �˸�
            AIStateManager aiManager = FindObjectOfType<AIStateManager>();
            if (aiManager != null)
            {
                aiManager.CheckSeed(); // AI���� ���ο� ������ Ȯ���ϵ��� ��û
            }
        }
        else
        {
            Debug.Log("������ ���� �� �����ϴ�: �̹� �ɾ��� �ֽ��ϴ�.");
        }
    }

    // ���� �ִ� �޼���
    public void WaterCrop()
    {
        // ������ �ɾ��� �ְ�, ���� �ܰ谡 0�̸� `NeedsWater` ������ ���� ���� �� �� �ֵ��� ����
        if (IsSeedPlanted() && currentStage == 0 && cropState == CropState.NeedsWater)
        {
            cropState = CropState.Watered;  // ���� �� ���·� ����
            growthStartTime = Time.time;     // ���� �� �������� ������ �����ϵ��� ����
            Debug.Log("���� �־����ϴ�. ������ �簳�մϴ�.");
        }
        else if (IsSeedPlanted() && cropState == CropState.Watered)
        {
            Debug.Log("�۹��� �̹� ���� �־������ϴ�.");
        }
        else
        {
            Debug.Log("���� ���� �� �� �����ϴ�: ������ �ɾ��� ���� �ʰų� ���� �ʿ����� ���� �����Դϴ�.");
        }
    }

    // �۹� ��Ȯ �޼��� �߰�
    public void Harvest()
    {
        if (IsReadyToHarvest())
        {
            Debug.Log("�۹��� ��Ȯ�߽��ϴ�.");
            cropState = CropState.Harvested;
            gameObject.SetActive(false);
            aiStateManager.AddToInventory(this);
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
        // ��Ȯ�� ��쿡�� �ƹ� �۾��� ���� ����
        if (cropState == CropState.ReadyToHarvest)
        {
            return;
        }

        if (cropState == CropState.Harvested)
        {
            return;
        }

        // ���� �ܰ谡 0�̰� `Watered` ���°� �ƴ� ��� ������ ���ߵ��� ����
        if (currentStage == 0 && cropState != CropState.Watered)
        {
            Debug.Log("0�ܰ迡�� ���� �ʿ��մϴ�. ���� �� ������ ������ ����ϴ�.");
            return; // ���� �� ������ ���� ����
        }

        // ���� �� ���� ���� �ܰ谡 1 �̻����� ����ǵ��� ����
        if (currentStage < growthStages.Length && currentTime - growthStartTime >= growthTimes[currentStage])
        {
            currentStage++;
            UpdateCropVisual();
            UpdateSortingLayer();
            Debug.Log($"���� ���� �ܰ�: {currentStage}");

            // ���� �ܰ谡 ������ �ܰ��� ��� ReadyToHarvest ���·� ����
            if (currentStage == 4)  // �迭 ������ �ܰ� Ȯ��
            {
                cropState = CropState.ReadyToHarvest;
                Debug.Log("�۹��� �� �ڶ� ��Ȯ�� �غ� �Ǿ����ϴ�.");
            }
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
