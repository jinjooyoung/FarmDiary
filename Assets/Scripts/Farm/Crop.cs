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
        Watered  // 물을 준 상태 추가
    }

    public CropState cropState;
    public SeedPlantedState seedPlantedState;

    public Vector2 seedPosition;

    public Grid grid;

    public bool isPreview;

    public GameObject[] growthStages; // 성장 단계별로 할당된 리소스 오브젝트들 (총 5개)
    private float[] growthTimes;  // 각 성장 단계에 필요한 시간
    public int currentStage = 0;  // 현재 작물의 성장 단계
    private float growthStartTime; // 성장이 시작된 시간

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
            Debug.LogError("Grid 시스템을 찾을 수 없습니다!");
            return;
        }

        // 현재 오브젝트의 월드 좌표에서 그리드 좌표로 변환
        SetFieldPosition(transform.position);
    }

    // 그리드 위치 설정 메서드
    public void SetFieldPosition(Vector3 worldPosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(worldPosition);

        // Vector3Int에서 x와 y 값을 추출하여 Vector2로 변환
        seedPosition = new Vector2(gridPosition.x, gridPosition.y);
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

    public bool IsReadyToHarvest()
    {
        return cropState == CropState.ReadyToHarvest;
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
            Destroy(gameObject);
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

        UpdateCropVisual();            // 초기 상태 업데이트
        UpdateSortingLayer();          // 초기 소팅 레이어 업데이트
    }

    // 각 단계별로 성장을 체크하고 성장 상태를 업데이트
    // CropGrowthManager 스크립트에서 호출됨
    public void CheckGrowth(float currentTime)
    {
        // 수확된 경우에는 아무 작업도 하지 않음
        if (cropState == CropState.ReadyToHarvest)
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
        if (currentStage < growthStages.Length && currentTime - growthStartTime >= growthTimes[currentStage])
        {
            currentStage++;
            UpdateCropVisual();
            UpdateSortingLayer();
            Debug.Log($"현재 성장 단계: {currentStage}");

            // 성장 단계가 마지막 단계인 경우 ReadyToHarvest 상태로 설정
            if (currentStage == 4)  // 배열 마지막 단계 확인
            {
                cropState = CropState.ReadyToHarvest;
                Debug.Log("작물이 다 자라서 수확할 준비가 되었습니다.");
            }
        }
    }

    // 작물의 상태를 업데이트
    private void UpdateCropVisual()
    {
        // 모든 성장 단계를 일단 비활성화
        for (int i = 0; i < growthStages.Length; i++)
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
    private void UpdateSortingLayer()
    {
        foreach (var stage in growthStages)
        {
            if (stage.activeSelf) // 현재 활성화된 성장 단계만 소팅 레이어 변경
            {
                SpriteRenderer renderer = stage.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingLayerName = "MiddleGround"; // 원하는 소팅 레이어 이름으로 변경
                    renderer.sortingOrder = CalculateSortingOrder(); // 계산된 소팅 오더로 설정
                }
            }
        }
    }

    // 위치에 따라 소팅 오더를 계산하는 메서드 (예시)
    private int CalculateSortingOrder()
    {
        return (int)(transform.position.y * -10); // Y값을 음수로 변환하여 정렬
    }
}
