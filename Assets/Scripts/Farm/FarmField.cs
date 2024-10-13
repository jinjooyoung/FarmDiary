using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmField : MonoBehaviour
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
        Watered  // 물을 준 상태 추가
    }
    
    public CropState cropState;
    public SeedPlantedState seedPlantedState;

    public Vector2 fieldPosition;

    public Grid grid;

    private AIStateManager aiStateManager;

    private void Awake()
    {
        aiStateManager = FindObjectOfType<AIStateManager>();

        if (aiStateManager != null)
        {
            aiStateManager.AddField(this);
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
        fieldPosition = new Vector2(gridPosition.x, gridPosition.y);
        Debug.Log($"밭 위치 설정됨: {fieldPosition}");
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
            cropState = CropState.NeedsWater;
            Debug.Log("물이 필요합니다");
        }
    }

    // 물을 주는 메서드 추가
    public void WaterCrop()
    {
        if (IsSeedPlanted() && NeedsWater())
        {
            cropState = CropState.Watered;  // 물을 준 상태로 변경
            Debug.Log("물을 줬습니다");
        }
        else if (IsSeedPlanted() && cropState == CropState.Watered)
        {
            Debug.Log("작물에 이미 물이 주어졌습니다.");
        }
        else if (IsSeedPlantedNo() && cropState == CropState.Empty)
        {
            Debug.Log("물을 줄 수 없습니다: 씨앗이 심어져 있지 않거나 물이 이미 뿌려져 있을 때");
        }
    }

    // 씨앗 심기 메서드
    public void PlantSeed()
    {
        // cropState가 Empty이고 seedPlantedState가 No일 때만 씨앗을 심을 수 있도록 변경
        if (cropState == CropState.Empty && seedPlantedState == SeedPlantedState.No)
        {
            cropState = CropState.SeedPlanted;
            seedPlantedState = SeedPlantedState.Yes;
            CheckSeedPlanted(); // 씨앗 심은 후 물이 필요한 상태로 설정
            Debug.Log("씨앗을 심었습니다.");
        }
        else
        {
            Debug.Log("씨앗을 심을 수 없습니다: 이미 심어져 있습니다.");
        }
    }
    
    // 작물 수확 메서드 추가
    public void Harvest()
    {
        if (IsReadyToHarvest())
        {
            cropState = CropState.Empty; // 수확 후 상태를 비워줌
            seedPlantedState = SeedPlantedState.No; // 씨앗 상태 초기화
            Debug.Log("작물을 수확했습니다.");
        }
        else
        {
            Debug.Log("작물을 수확할 수 없습니다: 수확할 준비가 되어 있지 않습니다.");
        }
    }
}
