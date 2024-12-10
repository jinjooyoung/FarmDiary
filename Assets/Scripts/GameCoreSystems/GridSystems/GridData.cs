using System.Collections.Generic;
using System;
using UnityEngine;
using static Crop;

[System.Serializable]
public class GridData
{
    // �׸��� ���� ��ġ(Vector3Int)�� Ű�� ����ϰ�, �� ��ġ�� ��ġ�� ������Ʈ�� ����
    public Dictionary<Vector3Int, PlacementData> placedFields = new();
    public Dictionary<Vector3Int, PlacementData> placedDecos = new();
    public Dictionary<Vector3Int, PlacementData> placedFacilities = new();
    public Dictionary<Vector3Int, PlacementData> placedCrops = new();

    // �����ϴ� ������Ʈ�� ���� ��ųʸ�����
    // 0 : �ʵ�, 1 : ����, 2 : �ü�, 3 : �۹�
    public int currentDictionary = -1;

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
        if (ID < 4) // �� ID�� �� ��ųʸ��� �߰�
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
        else if (ID > 100 && ID < 300)     // ���ڿ�����Ʈ ID�� ���� ��ųʸ��� �߰�
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
        if (!placedFields.ContainsKey(gridPosition) && !placedDecos.ContainsKey(gridPosition) && !placedFacilities.ContainsKey(gridPosition) && !placedCrops.ContainsKey(gridPosition))       // �ƹ��͵� ��ġ�Ǿ� ���� ���� ĭ�̶�� -1 ����
        {
            return -1;
        }

        if(placedFields.ContainsKey(gridPosition))
        {
            currentDictionary = 0;
            return placedFields[gridPosition].PlacedObjectIndex;
        }
        else if (placedDecos.ContainsKey(gridPosition))
        {
            currentDictionary = 1;
            return placedDecos[gridPosition].PlacedObjectIndex;
        }
        else if (placedFacilities.ContainsKey(gridPosition))
        {
            currentDictionary = 2;
            return placedFacilities[gridPosition].PlacedObjectIndex;
        }
        else
        {
            currentDictionary = 3;
            return placedCrops[gridPosition].PlacedObjectIndex;
        }
        
    }

    // ������ �׸��� ��ġ�� ���� �װ��� ��ġ�� �۹��� �ε����� ��ȯ�ϴ� �޼���
    internal int GetRepresentationIndexCrop(Vector3Int gridPosition)
    {
        if (!placedCrops.ContainsKey(gridPosition))       // �ƹ��͵� ��ġ�Ǿ� ���� ���� ĭ�̶�� -1 ����
        {
            return -1;
        }

        currentDictionary = 3;
        return placedCrops[gridPosition].PlacedObjectIndex;

    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (currentDictionary == 0)
        {
            foreach (var pos in placedFields[gridPosition].occupiedPositions)
            {
                placedFields.Remove(pos);
                placedCrops.Remove(pos);
            }
        }
        else if (currentDictionary == 1)
        {
            foreach (var pos in placedDecos[gridPosition].occupiedPositions)
            {
                placedDecos.Remove(pos);
            }
        }
        else if (currentDictionary == 2)
        {
            foreach (var pos in placedFacilities[gridPosition].occupiedPositions)
            {
                placedFacilities.Remove(pos);
            }
        }

    }

    internal void RemoveCropAt(Vector3Int gridPosition)
    {
        if (!placedCrops.ContainsKey(gridPosition))
        {
            return;
        }

        foreach (var pos in placedCrops[gridPosition].occupiedPositions)
        {
            placedCrops.Remove(pos);
        }
    }
}

[System.Serializable]
public class PlacementData      // ������Ʈ�� ���� Ŭ����
{
    // ������Ʈ�� �����ϴ� �׸��� ������ ��ǥ (������Ʈ ũ�Ⱑ Ŀ�� ���� ĭ�� ������ �� �־ ����Ʈ��)
    public List<Vector3Int> occupiedPositions;
    public int ID { get; set; }                 // ObjectsDataBase Scriptable Object ���� ���Ǵ� ������Ʈ�� ���� ID
    public int PlacedObjectIndex { get; private set; }  // ObjectsDataBase Scriptable Object ���� ������Ʈ�� �ε���

    /*// �۹� ���� �߰�
    public CropState cropState;
    public SeedPlantedState seedPlantedState;*/

    public float alpha;

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }

    // JSON ������ ���� �⺻ ������ �߰�
    public PlacementData() { }
}