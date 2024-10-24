using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropPlacementState : IBuildingState
{
    private int selectedCropIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData placedCropData;
    OBJPlacer cropPlacer;

    public CropPlacementState(int id,
                              Grid grid,
                              PreviewSystem previewSystem,
                              ObjectsDatabaseSO database,
                              GridData placedCropData,
                              OBJPlacer cropPlacer)
    {
        ID = id;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.placedCropData = placedCropData;
        this.cropPlacer = cropPlacer;

        selectedCropIndex = database.objectsData.FindIndex(data => data.ID == ID);    // 설치할 작물 선택

        if (selectedCropIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedCropIndex].Prefab,
                database.objectsData[selectedCropIndex].Size);
        }
        else
        {
            throw new System.Exception($"No crop with ID {id}");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (!IsFieldPlaced(gridPosition) || IsCropPlaced(gridPosition))    // 해당 위치에 밭이 없거나 작물이 있으면
        {
            return;
        }

        int index = cropPlacer.PlaceObject(database.objectsData[selectedCropIndex].Prefab, grid.CellToWorld(gridPosition));

        // CropGrowthManager에 등록
        GameObject placedCrop = cropPlacer.placedGameObjects[index];    // 설치된 오브젝트 가져오기
        Crop cropScript = placedCrop.GetComponent<Crop>();              // Crop 스크립트 가져오기

        if (cropScript != null)
        {
            cropScript.Initialize(database.objectsData[selectedCropIndex].GrowthTimes);

            // CropGrowthManager에 등록
            CropGrowthManager.Instance.RegisterCrop(cropScript);
        }

        placedCropData.AddCropAt(gridPosition, database.objectsData[selectedCropIndex].Size,
            database.objectsData[selectedCropIndex].ID, index);

        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), false);
    }

    private bool IsFieldPlaced(Vector3Int gridPosition) // 해당 위치에 밭이 존재하면 true
    {
        return placedCropData.placedFields.ContainsKey(gridPosition);
    }

    private bool IsCropPlaced(Vector3Int gridPosition)  // 해당 위치에 작물이 존재하면 true
    {
        return placedCropData.placedCrops.ContainsKey(gridPosition);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity;
        if (!IsFieldPlaced(gridPosition) || IsCropPlaced(gridPosition))    // 해당 위치에 밭이 없거나 작물이 있으면 = 심을 수 없으면
        {
            placementValidity = false;
        }
        else
        {
            placementValidity = true;
        }
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), placementValidity);
    }
}