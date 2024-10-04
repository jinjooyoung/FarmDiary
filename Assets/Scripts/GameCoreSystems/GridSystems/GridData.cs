using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    // �׸��� ���� ��ġ(Vector3Int)�� Ű�� ����ϰ�, �� ��ġ�� ��ġ�� ������Ʈ�� ����
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
            }
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, y, 0));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }
}

public class PlacementData      // ������Ʈ�� ���� Ŭ����
{
    // ������Ʈ�� �����ϴ� �׸��� ������ ��ǥ (������Ʈ ũ�Ⱑ Ŀ�� ���� ĭ�� ������ �� �־ ����Ʈ��)
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }                 // ObjectsDataBase Scriptable Object ���� ���Ǵ� ������Ʈ�� ���� ID
    public int PlacedObjectIndex { get; private set; }  // ObjectsDataBase Scriptable Object ���� ������Ʈ�� �ε���

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
