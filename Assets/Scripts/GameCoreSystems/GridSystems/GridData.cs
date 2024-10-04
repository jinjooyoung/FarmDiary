using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    // 그리드 셀의 위치(Vector3Int)를 키로 사용하고, 그 위치에 배치된 오브젝트의 정보
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
                throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
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

public class PlacementData      // 오브젝트의 정보 클래스
{
    // 오브젝트가 차지하는 그리드 셀들의 좌표 (오브젝트 크기가 커서 여러 칸을 차지할 수 있어서 리스트로)
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }                 // ObjectsDataBase Scriptable Object 에서 사용되는 오브젝트의 고유 ID
    public int PlacedObjectIndex { get; private set; }  // ObjectsDataBase Scriptable Object 에서 오브젝트의 인덱스

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
