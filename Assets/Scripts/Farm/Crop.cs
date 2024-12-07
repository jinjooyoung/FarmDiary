using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작물의 정보가 들어있는 클래스
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
        Watered  // 물을 준 상태 추가
    }

    public int ID = -1;
    public int PlacedObjectIndex; // 오브젝트의 인덱스
    public List<Vector3Int> occupiedPositions; // 이 작물이 차지하는 그리드 좌표

    public int sellPrice = 0;

    public CropState cropState;
    public SeedPlantedState seedPlantedState;

    public Vector3Int seedPosition;

    public Grid grid;

    public bool isPreview;

    public GameObject[] growthStages; // 성장 단계별로 할당된 리소스 오브젝트들 (총 5개)
    public float[] growthTimes;  // 각 성장 단계에 필요한 시간
    public int currentStage = 0;  // 현재 작물의 성장 단계
    public float growthStartTime; // 성장이 시작된 시간

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
        occupiedPositions = new List<Vector3Int>(); // 빈 리스트로 초기화

        grid = FindObjectOfType<Grid>();

        if (grid == null)
        {
            Debug.LogError("Grid 시스템을 찾을 수 없습니다!");
            return;
        }

        // 현재 오브젝트의 월드 좌표에서 그리드 좌표로 변환
        SetFieldPosition(transform.position);
    }

    // 그리드 위치 설정 메서드
    public void SetFieldPosition(Vector3 worldPosition)
    {
        seedPosition = grid.WorldToCell(worldPosition);
        Debug.Log($"밭 위치 설정됨: {seedPosition}");
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
            cropState = CropState.NeedsWater; // 씨앗이 심어지면 물이 필요한 상태로 설정
            Debug.Log("씨앗이 심어졌습니다. 물이 필요합니다.");
        }
    }

    // 씨앗 심기 메서드
    public void PlantSeed()
    {
        if (!isPreview)
        {
            cropState = CropState.NeedsWater; // 심은 후 물이 필요하게 설정
            seedPlantedState = SeedPlantedState.Yes;
            currentStage = 0;  // 초기 성장 단계 설정
            growthStartTime = 0; // 초기화하여 성장이 멈추도록 설정

            Debug.Log("씨앗을 심었습니다. 물이 필요합니다.");

            // AI에게 새로운 씨앗을 확인하도록 알림
            AIStateManager aiManager = FindObjectOfType<AIStateManager>();
            if (aiManager != null)
            {
                aiManager.CheckSeed(); // AI에게 새로운 씨앗을 확인하도록 요청
            }
        }
        else
        {
            Debug.Log("씨앗을 심을 수 없습니다: 이미 심어져 있습니다.");
        }
    }

    // 물을 주는 메서드
    public void WaterCrop()
    {
        // 씨앗이 심어져 있고, 현재 단계가 0이며 `NeedsWater` 상태일 때만 물을 줄 수 있도록 설정
        if (IsSeedPlanted() && currentStage == 0 && cropState == CropState.NeedsWater)
        {
            cropState = CropState.Watered;  // 물을 준 상태로 변경
            growthStages[5].SetActive(true);
            growthStartTime = Time.time;     // 물을 준 순간부터 성장을 시작하도록 설정
            Debug.Log("물을 주었습니다. 성장을 재개합니다.");
        }
        else if (IsSeedPlanted() && cropState == CropState.Watered)
        {
            Debug.Log("작물에 이미 물이 주어졌습니다.");
        }
        else
        {
            Debug.Log("현재 물을 줄 수 없습니다: 씨앗이 심어져 있지 않거나 물이 필요하지 않은 상태입니다.");
        }
    }

    // 작물 수확 메서드 추가
    public void Harvest()
    {
        if (IsReadyToHarvest())
        {
            Debug.Log("작물을 수확했습니다.");
            cropState = CropState.Harvested;
            Destroy(gameObject);
            GameManager.AddCoins(sellPrice);
            aiStateManager.AddToInventory(this);
        }
        else
        {
            Debug.Log("작물을 수확할 수 없습니다: 수확할 준비가 되어 있지 않습니다.");
        }
    }

    // 성장 시간을 초기화하고, 첫 성장 단계를 설정
    public void Initialize(float[] cropGrowthTimes)
    {
        growthTimes = cropGrowthTimes;
        growthStartTime = Time.time;  // 성장이 시작된 시간을 저장
        currentStage = 0;             // 초기 성장 단계 설정

        UpdateSortingLayer();          // 초기 소팅 레이어 업데이트
        UpdateCropVisual();            // 초기 상태 업데이트
        growthStages[5].SetActive(false);   // 물 텍스쳐 처음에는 꺼짐
    }

    // 각 단계별로 성장을 체크하고 성장 상태를 업데이트
    // CropGrowthManager 스크립트에서 호출됨
    public void CheckGrowth(float currentTime)
    {
        if (currentStage >= growthTimes.Length)
        {
            Debug.LogWarning($"currentStage ({currentStage})가 growthTimes 배열 크기 ({growthTimes.Length})를 초과했습니다. 초기화가 필요합니다.");
            return; // 배열 범위를 벗어나면 함수 종료
        }

        // 수확된 경우에는 아무 작업도 하지 않음
        if (cropState == CropState.ReadyToHarvest)      // 수확 대기상태
        {
            return;
        }

        if (cropState == CropState.Harvested)           // 수확됨 상태
        {
            return;
        }

        // 성장 단계가 0이고 `Watered` 상태가 아닐 경우 성장을 멈추도록 설정
        if (currentStage == 0 && cropState != CropState.Watered)
        {
            Debug.Log("0단계에서 물이 필요합니다. 물을 줄 때까지 성장을 멈춥니다.");
            return; // 물을 줄 때까지 성장 멈춤
        }

        // 물을 준 이후 성장 단계가 1 이상으로 진행되도록 설정
        if (currentStage < growthTimes.Length && currentTime - growthStartTime >= growthTimes[currentStage])
        {
            currentStage++;
            UpdateCropVisual();

            Debug.Log($"현재 성장 단계: {currentStage}");

            // 마지막 단계일 경우 ReadyToHarvest로 설정
            if (currentStage >= growthTimes.Length - 1)
            {
                currentStage = growthTimes.Length - 1; // 안전하게 마지막 단계로 고정
                cropState = CropState.ReadyToHarvest;
                Debug.Log("작물이 다 자라서 수확할 준비가 되었습니다.");
            }
        }
    }

    private void OnDestroy()    // 수확하여 파괴되면 호출
    {
        CropGrowthManager.Instance.crops.Remove(this);
        CropGrowthManager.Instance.cropsPos.Remove(seedPosition);
    }

    // 작물의 상태를 업데이트
    public void UpdateCropVisual()
    {
        // 모든 성장 단계를 일단 비활성화
        for (int i = 0; i < 5; i++)
        {
            growthStages[i].SetActive(false);
        }

        // 현재 단계에 해당하는 오브젝트만 활성화
        if (currentStage >= 0 && currentStage < 5)
        {
            growthStages[currentStage].SetActive(true);
        }
    }

    // 소팅 레이어 업데이트
    public void UpdateSortingLayer()
    {
        for (int i = 0; i < 5; i++)
        {
            SpriteRenderer renderer = growthStages[i].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "MiddleGround"; // 원하는 소팅 레이어 이름으로 변경
                renderer.sortingOrder = CalculateSortingOrder(); // 계산된 소팅 오더로 설정
            }
        }
    }

    // 위치에 따라 소팅 오더를 계산하는 메서드 (예시)
    private int CalculateSortingOrder()
    {
        return (int)(transform.position.y * -10); // Y값을 음수로 변환하여 정렬
    }

    public void LoadPlacementData(PlacementData placementData)
    {
        Debug.Log($"로드된 ID: {placementData.ID}, 위치: {placementData.occupiedPositions}");
        this.ID = placementData.ID;
        this.PlacedObjectIndex = placementData.PlacedObjectIndex;
        this.cropState = placementData.cropState;
        this.seedPlantedState = placementData.seedPlantedState;
        this.occupiedPositions = placementData.occupiedPositions;
    }
}