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

    public int ID = -1;
    public int PlacedObjectIndex; // ������Ʈ�� �ε���
    public List<Vector3Int> occupiedPositions; // �� �۹��� �����ϴ� �׸��� ��ǥ

    public int sellPrice = 0;

    public CropState cropState;
    public SeedPlantedState seedPlantedState;

    public Vector3Int seedPosition;

    public Grid grid;

    public bool isPreview;

    public GameObject[] growthStages; // ���� �ܰ躰�� �Ҵ�� ���ҽ� ������Ʈ�� (�� 5��)
    public float[] growthTimes;  // �� ���� �ܰ迡 �ʿ��� �ð�
    public int currentStage = 0;  // ���� �۹��� ���� �ܰ�
    public float growthStartTime; // ������ ���۵� �ð�

    private AIStateManager aiStateManager;

    private GameManager gameManager;

    private void Awake()
    {
        aiStateManager = FindObjectOfType<AIStateManager>();

        if (aiStateManager != null)
        {
            //aiStateManager.AddSeed(this);
        }

        gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            gameManager.AddSeed(this);
        }
    }

    private void Start()
    {
        occupiedPositions = new List<Vector3Int>(); // �� ����Ʈ�� �ʱ�ȭ

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
        seedPosition = grid.WorldToCell(worldPosition);
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
            growthStages[5].SetActive(true);
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
            Destroy(gameObject);
            GameManager.AddCoins(sellPrice);
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

        UpdateSortingLayer();          // �ʱ� ���� ���̾� ������Ʈ
        UpdateCropVisual();            // �ʱ� ���� ������Ʈ
        growthStages[5].SetActive(false);   // �� �ؽ��� ó������ ����
    }

    // �� �ܰ躰�� ������ üũ�ϰ� ���� ���¸� ������Ʈ
    // CropGrowthManager ��ũ��Ʈ���� ȣ���
    public void CheckGrowth(float currentTime)
    {
        if (currentStage >= growthTimes.Length)
        {
            Debug.LogWarning($"currentStage ({currentStage})�� growthTimes �迭 ũ�� ({growthTimes.Length})�� �ʰ��߽��ϴ�. �ʱ�ȭ�� �ʿ��մϴ�.");
            return; // �迭 ������ ����� �Լ� ����
        }

        // ��Ȯ�� ��쿡�� �ƹ� �۾��� ���� ����
        if (cropState == CropState.ReadyToHarvest)      // ��Ȯ ������
        {
            return;
        }

        if (cropState == CropState.Harvested)           // ��Ȯ�� ����
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
        if (currentStage < growthTimes.Length && currentTime - growthStartTime >= growthTimes[currentStage])
        {
            currentStage++;
            UpdateCropVisual();

            Debug.Log($"���� ���� �ܰ�: {currentStage}");

            // ������ �ܰ��� ��� ReadyToHarvest�� ����
            if (currentStage >= growthTimes.Length - 1)
            {
                currentStage = growthTimes.Length - 1; // �����ϰ� ������ �ܰ�� ����
                cropState = CropState.ReadyToHarvest;
                Debug.Log("�۹��� �� �ڶ� ��Ȯ�� �غ� �Ǿ����ϴ�.");
            }
        }
    }

    private void OnDestroy()    // ��Ȯ�Ͽ� �ı��Ǹ� ȣ��
    {
        CropGrowthManager.Instance.crops.Remove(this);
        CropGrowthManager.Instance.cropsPos.Remove(seedPosition);
    }

    // �۹��� ���¸� ������Ʈ
    public void UpdateCropVisual()
    {
        // ��� ���� �ܰ踦 �ϴ� ��Ȱ��ȭ
        for (int i = 0; i < 5; i++)
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
    public void UpdateSortingLayer()
    {
        for (int i = 0; i < 5; i++)
        {
            SpriteRenderer renderer = growthStages[i].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "MiddleGround"; // ���ϴ� ���� ���̾� �̸����� ����
                renderer.sortingOrder = CalculateSortingOrder(); // ���� ���� ������ ����
            }
        }
    }

    // ��ġ�� ���� ���� ������ ����ϴ� �޼��� (����)
    private int CalculateSortingOrder()
    {
        return (int)(transform.position.y * -10); // Y���� ������ ��ȯ�Ͽ� ����
    }

    public void LoadPlacementData(PlacementData placementData)
    {
        Debug.Log($"�ε�� ID: {placementData.ID}, ��ġ: {placementData.occupiedPositions}");
        this.ID = placementData.ID;
        this.PlacedObjectIndex = placementData.PlacedObjectIndex;
        this.cropState = placementData.cropState;
        this.seedPlantedState = placementData.seedPlantedState;
        this.occupiedPositions = placementData.occupiedPositions;
    }
}