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

        selectedCropIndex = database.objectsData.FindIndex(data => data.ID == ID);    // ��ġ�� �۹� ����

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
        if (!IsFieldPlaced(gridPosition) || IsCropPlaced(gridPosition))    // �ش� ��ġ�� ���� ���ų� �۹��� ������
        {
            return;
        }

        int index = cropPlacer.PlaceObject(database.objectsData[selectedCropIndex].Prefab, grid.CellToWorld(gridPosition));

        // CropGrowthManager�� ���
        GameObject placedCrop = cropPlacer.placedGameObjects[index];    // ��ġ�� ������Ʈ ��������
        Crop cropScript = placedCrop.GetComponent<Crop>();              // Crop ��ũ��Ʈ ��������

        if (cropScript != null)
        {
            cropScript.Initialize(database.objectsData[selectedCropIndex].GrowthTimes);

            // CropGrowthManager�� ���
            CropGrowthManager.Instance.RegisterCrop(cropScript);
        }

        placedCropData.AddCropAt(gridPosition, database.objectsData[selectedCropIndex].Size,
            database.objectsData[selectedCropIndex].ID, index);

        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), false);
    }

    private bool IsFieldPlaced(Vector3Int gridPosition) // �ش� ��ġ�� ���� �����ϸ� true
    {
        return placedCropData.placedFields.ContainsKey(gridPosition);
    }

    private bool IsCropPlaced(Vector3Int gridPosition)  // �ش� ��ġ�� �۹��� �����ϸ� true
    {
        return placedCropData.placedCrops.ContainsKey(gridPosition);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity;
        if (!IsFieldPlaced(gridPosition) || IsCropPlaced(gridPosition))    // �ش� ��ġ�� ���� ���ų� �۹��� ������ = ���� �� ������
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