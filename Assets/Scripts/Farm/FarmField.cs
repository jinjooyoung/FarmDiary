using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using static Crop; // Crop 클래스 내부의 열거형 참조

public class FarmField : MonoBehaviour
{
    private GameManager gameManager;

    public bool isPreview;

    public int ID;
    public int PlacedObjectIndex;
    public CropState cropState; // 작물 상태
    public SeedPlantedState seedPlantedState; // 씨앗 상태
    public List<Vector3Int> occupiedPositions;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.Addfield(this);
        }
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