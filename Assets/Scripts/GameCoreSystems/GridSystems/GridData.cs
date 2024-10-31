using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    // �׸��� ���� ��ġ(Vector3Int)�� Ű�� ����ϰ�, �� ��ġ�� ��ġ�� ������Ʈ�� ����
    public Dictionary<Vector3Int, PlacementData> placedFields = new();
    public Dictionary<Vector3Int, PlacementData> placedDecos = new();
    public Dictionary<Vector3Int, PlacementData> placedFacilities = new();
    public Dictionary<Vector3Int, PlacementData> placedCrops = new();

    // ��ġ�� ������Ʈ�� ������ �����ϰ� �� �� �� ��ġ �������� üũ�ϴ� �޼���
    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        // ��ġ�� ��ġ�� �׸��� �������� �޾ƿͼ� ����Ʈ�� ����
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        // ��ġ�� ������Ʈ�� ������ ����
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        // foreach �ݺ��� ���鼭 �̹� ������ ĭ�� �ִ��� Ȯ��
        // ��¥�� �� �Լ��� ȣ���ϴ� �������� ��ġ ���� ������ üũ������ �� �� �� üũ�����μ� Ȥ�� �� ���׸� ����
        if (ID < 5) // �� ID�� �� ��ųʸ��� �߰�
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedFields.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedDecos.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedFacilities.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                placedFields[pos] = data;
                // ������Ʈ�� �����ϴ� ĭ�� pos���� ������Ʈ�� ������ ��ųʸ��� ����
            }
        }
        else if (ID >= 200 && ID < 300)     // ���ڿ�����Ʈ ID�� ���� ��ųʸ��� �߰�
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedFields.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedDecos.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedFacilities.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                placedDecos[pos] = data;
                // ������Ʈ�� �����ϴ� ĭ�� pos���� ������Ʈ�� ������ ��ųʸ��� ����
            }
        }
        else  // �� ���� ID�� ��� ������Ʈ�̹Ƿ� ��� ��ųʸ��� ���� (�۹� ID�� ��¥�� �ؿ��� ȣ���ϱ⶧����)
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedFields.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedDecos.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedFacilities.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                placedFacilities[pos] = data;
                // ������Ʈ�� �����ϴ� ĭ�� pos���� ������Ʈ�� ������ ��ųʸ��� ����
            }
        }
    }

    // ��ġ�� �۹��� ������ �����ϰ� �� �� �� ��ġ �������� üũ�ϴ� �޼���
    public void AddCropAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        // ��ġ�� ��ġ�� �׸��� �������� �޾ƿͼ� ����Ʈ�� ����
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        // ��ġ�� ������Ʈ�� ������ ����
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        // foreach �ݺ��� ���鼭 �̹� ������ ĭ�� �ִ��� Ȯ��
        // ��¥�� �� �Լ��� ȣ���ϴ� �������� ��ġ ���� ������ üũ������ �� �� �� üũ�����μ� Ȥ�� �� ���׸� ����
        foreach (var pos in positionToOccupy)
        {
            if (placedCrops.ContainsKey(pos))
            {
                throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
            }
            placedCrops[pos] = data;
            // ������Ʈ�� �����ϴ� ĭ�� pos���� ������Ʈ�� ������ ��ųʸ��� ����
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

    // ������Ʈ�� ����� �޾ƿͼ� �� ��ġ�� ��ġ �������� bool�� ����
    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        // ������Ʈ�� ������(���� ��ġ X ����) �׸��� ĭ�� ��ġ�� ����Ʈ�� ����
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);

        // ����� ĭ���� ���鼭 �� ��ġ�� �̹� �ٸ� ������Ʈ�� �����Ǿ� �ִ��� ���
        foreach (var pos in positionToOccupy)
        {
            if (placedFields.ContainsKey(pos) || pos.y > -7 || placedDecos.ContainsKey(pos) || placedFacilities.ContainsKey(pos))     // �� ĭ�̶� �̹� ������ ��ġ�� ������ false�� ��ȯ�Ͽ� ��ġ �Ұ���
            {
                return false;
            }
            //Debug.Log(pos);
        }
        return true;        // ��ġ �����̸� true ������
    }

    // ������ �׸��� ��ġ�� ���� �װ��� ��ġ�� ������Ʈ�� �ε����� ��ȯ�ϴ� �޼���
    // ���⿡�� ���ϴ� ������Ʈ �ε����� ��ũ���ͺ������Ʈ�� �ε����� �ƴ� GridData�� ��ġ�� �� ���� �߰��Ǵ� ����Ʈ�� �ε���
    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedFields.ContainsKey(gridPosition) == false)       // �ƹ��͵� ��ġ�Ǿ� ���� ���� ĭ�̶�� -1 ����
        {
            return -1;
        }
        return placedFields[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedFields[gridPosition].occupiedPositions)
        {
            placedFields.Remove(pos);
            placedCrops.Remove(pos);
        }
    }

    internal void RemoveCropAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedCrops[gridPosition].occupiedPositions)
        {
            placedCrops.Remove(pos);
        }
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
