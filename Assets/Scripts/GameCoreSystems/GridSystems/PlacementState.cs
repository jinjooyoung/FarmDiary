using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IBuildingState�� Ȱ���Ͽ� ������Ʈ�� ���� ID�� �޾ƿ��� �ٸ� ������ ���� ���� �� �ְ� �ϴ� ��?
// ó�� �Ẹ�� ����̶� Ȯ������ X �� �𸣰���
public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData placedOBJData;
    OBJPlacer objectPlacer;

    public PlacementState(int id,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData placedOBJData,
                          OBJPlacer objectPlacer)
    {
        ID = id;
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
        if (placementValidity == false || GameManager.currentCoin < database.objectsData[selectedObjectIndex].BuyPrice)     // ��ġ�� �Ұ����� ��
        {
            return;
        }

        if (ID == 4 && objectPlacer.potCount >= 5)      // ���� ��ġ�� �� �̹� ���� 5�� �̻� �ִٸ� ��ġ �Ұ���
        {
            return;
        }

        // ���������� ��ġ�� ������Ʈ�� �ε���(������ �ε��� ����)
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));

        GridData selectedData = placedOBJData;      // ������ ������Ʈ�� ������ �޾ƿ�
        // GridData ��ũ��Ʈ�� ����� placedObjects ��ųʸ��� �޾ƿ� ������ ����
        selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID, index);

        GameManager.SubtractCoins(database.objectsData[selectedObjectIndex].BuyPrice);

        if (ID == 4 && objectPlacer.potCount < 5)       // ���� ��ġ�� �� ���� 5�� ���϶�� ��ġ ���� ���� �� ���� ����, ���� ����
        {
            objectPlacer.potCount++;
            ObjectsDatabase.PriceIncrease(4);
            UIManager.instance.Pot.text = ObjectsDatabase.CurrentPrice(4).ToString();
        }

        /*// ������Ʈ�� ID�� ���� �� ������Ʈ��� �ش� �� ������ ������Ŵ
        switch (ID)
        {
            case 0:
                ObjectsDatabase.PriceIncrease(0);
                UIManager.instance.one.text = ObjectsDatabase.CurrentPrice(0).ToString();
                break;
            case 1:
                ObjectsDatabase.PriceIncrease(1);
                UIManager.instance.two.text = ObjectsDatabase.CurrentPrice(1).ToString();
                break;
            case 2:
                ObjectsDatabase.PriceIncrease(2);
                UIManager.instance.three.text = ObjectsDatabase.CurrentPrice(2).ToString();
                break;
            case 3:
                ObjectsDatabase.PriceIncrease(3);
                UIManager.instance.four.text = ObjectsDatabase.CurrentPrice(3).ToString();
                break;
        }*/

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
