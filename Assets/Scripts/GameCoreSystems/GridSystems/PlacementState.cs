using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IBuildingState�� Ȱ���Ͽ� ������Ʈ�� ���� ID�� �޾ƿ��� �ٸ� ������ ���� ���� �� �ְ� �ϴ� ��?
// ó�� �Ẹ�� ����̶� Ȯ������ X �� �𸣰���
public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    PlacementData.Category objectCategory;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData placedOBJData;
    OBJPlacer objectPlacer;

    public PlacementState(int id, PlacementData.Category objectCategory,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData placedOBJData,
                          OBJPlacer objectPlacer)
    {
        ID = id;
        this.objectCategory = objectCategory;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.placedOBJData = placedOBJData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);    // ��ġ�� ������Ʈ ����

        if (selectedObjectIndex > -1)       // ��ġ�� ������Ʈ�� �Ҵ�Ǿ� ã�Ҵٸ�
        {
            // ������ ����
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with ID {id}");
        }
    }

    public void EndState()      // ������ ���� ����
    {
        previewSystem.StopShowingPreview();
    }

    // PlaceObject �޼��带 ���� ��ġ
    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = GetPlacementValidity(gridPosition, selectedObjectIndex);   // ��ġ ����/�Ұ��� bool�� �޾ƿ�
        if (placementValidity == false)     // ��ġ�� �Ұ����� ��
        {
            return;
        }
        // ���������� ��ġ�� ������Ʈ�� �ε���(������ �ε��� ����)
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));

        GridData selectedData = placedOBJData;      // ������ ������Ʈ�� ������ �޾ƿ�
        // GridData ��ũ��Ʈ�� ����� placedObjects ��ųʸ��� �޾ƿ� ������ ����
        selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, objectCategory,
            database.objectsData[selectedObjectIndex].ID, index);

        // ������ ������Ʈ (�� PlaceObject �޼��� ȣ��������� ������Ʈ�� ��ġ�����Ƿ� ���� ��ġ �Ұ���.
        // ���� false�� Ŀ�� UI�� ���������� ������Ʈ)
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), false);
    }

    // ���õ� (��ġ�� ������) ������Ʈ�� ����� �޾ƿͼ� CanPlaceObjectAt�� �Ѱ�, ��ġ �������� bool�� ����
    private bool GetPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        return placedOBJData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    // ������Ʈ ������Ʈ
    public void UpdateState(Vector3Int gridPosition)
    {
        // ���õ� ������Ʈ�� �޾ƿͼ� ũ�⸦ ���� ��ġ ����/�Ұ��� �Ǻ�
        bool placementValidity = GetPlacementValidity(gridPosition, selectedObjectIndex);

        // ������ ������Ʈ
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), placementValidity);
    }
}
