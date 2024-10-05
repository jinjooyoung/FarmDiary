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

    public bool IsSeedPlanted(Vector2 position)
    {
        return seedPlantedState == SeedPlantedState.Yes;
    }
    
    public bool IsSeedPlantedNo(Vector2 position)
    {
        return seedPlantedState == SeedPlantedState.No;
    }
    
    public bool NeedsWater(Vector2 position)
    {
        return cropState == CropState.NeedsWater;
    }
    
    public bool IsReadyToHarvest(Vector2 position)
    {
        return cropState == CropState.ReadyToHarvest;
    }

    public void CheckSeedPlanted(Vector2 position)
    {
        if (seedPlantedState == SeedPlantedState.Yes)
        {
            cropState = CropState.NeedsWater;
            Debug.Log("물이 필요합니다");
        }
    }

    // 물을 주는 메서드 추가
    public void WaterCrop(Vector2 position)
    {
        if (IsSeedPlanted(position) && NeedsWater(position))
        {
            cropState = CropState.Watered;  // 물을 준 상태로 변경
            Debug.Log("물을 줬습니다");
        }
        else if (IsSeedPlanted(position) && cropState == CropState.Watered)
        {
            Debug.Log("작물에 이미 물이 주어졌습니다.");
        }
        else if (IsSeedPlantedNo(position) && cropState == CropState.Empty)
        {
            Debug.Log("물을 줄 수 없습니다: 씨앗이 심어져 있지 않거나 물이 이미 뿌려져 있을 때");
        }
    }

    // 씨앗 심기 메서드
    public void PlantSeed(Vector2 position)
    {
        // cropState가 Empty이고 seedPlantedState가 No일 때만 씨앗을 심을 수 있도록 변경
        if (cropState == CropState.Empty && seedPlantedState == SeedPlantedState.No)
        {
            cropState = CropState.SeedPlanted;
            seedPlantedState = SeedPlantedState.Yes;
            CheckSeedPlanted(position); // 씨앗 심은 후 물이 필요한 상태로 설정
            Debug.Log("씨앗을 심었습니다.");
        }
        else
        {
            Debug.Log("씨앗을 심을 수 없습니다: 이미 심어져 있습니다.");
        }
    }
    
    // 작물 수확 메서드 추가
    public void Harvest(Vector2 position)
    {
        if (IsReadyToHarvest(position))
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
