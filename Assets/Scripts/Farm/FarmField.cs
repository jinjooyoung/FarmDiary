/*using static Crop;
using System.Collections.Generic;
using UnityEngine;

public class FarmField : MonoBehaviour
{
    private GameManager gameManager;

    public bool isPreview;

    public int ID;
    public int PlacedObjectIndex;
    public CropState cropState; // 작물 상태
    public SeedPlantedState seedPlantedState; // 씨앗 상태
    public List<Vector3Int> occupiedPositions;

    public Grid grid;

    public Vector2 fieldPos;

    public Crop PlantedCrop; // 밭에 심어진 작물 참조

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.Addfield(this);
        }

        grid = FindObjectOfType<Grid>();

        if (grid == null)
        {
            Debug.LogError("Grid 시스템을 찾을 수 없습니다!");
            return;
        }

        // 현재 오브젝트의 월드 좌표에서 그리드 좌표로 변환
        SetFieldPosition(transform.position);
    }

    public void SetFieldPosition(Vector3 worldPosition)
    {
        // 그리드 좌표로 변환
        Vector3Int gridPosition = grid.WorldToCell(worldPosition);
        fieldPos = new Vector2(gridPosition.x, gridPosition.y);

        Debug.Log($"밭 위치 설정: 월드 위치={worldPosition}, 그리드 위치={gridPosition}, seedPosition={fieldPos}");
    }

    public void SetGrid(Grid grid)
    {
        this.grid = grid;
    }

    public void SetPlantedCrop(Crop crop, GridCropSave cropSaveData)
    {
        this.PlantedCrop = crop;

        // 작물 데이터 복원
        if (cropSaveData != null)
        {
            crop.LoadPlacementData(cropSaveData.placementData);
            crop.ID = cropSaveData.id;
            crop.currentStage = cropSaveData.currentStage; // 성장 단계 복원
            crop.cropState = cropSaveData.cropState; // 상태 복원
            crop.growthStartTime = cropSaveData.growthStartTime;

            crop.UpdateCropVisual(); // 시각적 상태 업데이트
            crop.UpdateSortingLayer(); // 소팅 레이어 복원

            // 자식 SpriteRenderer의 알파 값 복원
            SpriteRenderer[] childRenderers = crop.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in childRenderers)
            {
                Color color = spriteRenderer.color;
                color.a = cropSaveData.placementData.alpha; // 저장된 알파 값 적용
                spriteRenderer.color = color;
            }
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
}*/
